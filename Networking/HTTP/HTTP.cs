using System;
using BorrehSoft.ApolloGeese.CoreTypes;
using System.Text;
using BorrehSoft.Utilities.Collections.Settings;
using BorrehSoft.Utilities.Collections.Maps;
using BorrehSoft.Utilities.Log;
using System.Net;
using System.Collections.Generic;

namespace Networking
{
    public class HTTP : TwoBranchedService
    {
        /// <summary>
        /// The valid HTTP methods.
        /// </summary>
        public static readonly string[] ValidMethods = new string[] {
            "OPTIONS", "GET", "HEAD", "POST", "PUT", "DELETE", "TRACE", "CONNECT"
        };

        /// <summary>
        /// Gets or sets the URL encoding.
        /// </summary>
        /// <value>The URL encoding.</value>
        Encoding UrlEncoding { get; set; }

        /// <summary>
        /// Gets or sets the Request MIME type
        /// </summary>
        /// <value>The type of the MIME.</value>
        object MimeType { get; set; }
        public string ProxyServerVariable { get; private set; }

        Settings HeaderMapping = new Settings();

        /// <summary>
        /// Gets or sets the default URI
        /// </summary>
        /// <value>The default URI</value>
        protected string DefaultURI { get; set; }

        /// <summary>
        /// Gets or sets the URI Composition Service.
        /// </summary>
        /// <value>The UR.</value>
        protected Service URI { get; set; }

        /// <summary>
        /// Gets or sets the body composition Service.
        /// </summary>
        /// <value>The body.</value>
        protected Service Body { get; set; }

        /// <summary>
        /// Gets or sets the request method.
        /// </summary>
        /// <value>The method.</value>
        string Method { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance validates the HTTP method strictly.
        /// </summary>
        /// <value><c>true</c> if this instance is method validated strictly; otherwise, <c>false</c>.</value>
        bool IsMethodValidatedStrictly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance uses HTTP authentication.
        /// </summary>
        /// <value><c>true</c> if use authentication; otherwise, <c>false</c>.</value>
        bool UseAuthentication { get; set; }

        public override void LoadDefaultParameters(string defaultParameter)
        {
            this.Settings["method"] = defaultParameter;
        }

        public override string Description {
            get {
                string uri;
                if (TryProduceURI(null, out uri)) {
                    return string.Format("{0} Request for {1}",
                        this.Method, uri);
                } else {
                    return string.Format("{0} Request for undeterminable URI");
                }
            }
        }

        public bool UseBearer { get; private set; }
        public string BearerToken { get; private set; }
        public bool UseBearerVar { get; private set; }
        public string BearerVar { get; private set; }

        protected override void Initialize(Settings settings)
        {
            this.UrlEncoding = Encoding.GetEncoding(settings.GetString("urlencoding", "utf-8"));
            this.DefaultURI = settings.GetString("uri", "");
            this.Method = settings.GetString("method", "GET");
            this.IsMethodValidatedStrictly = settings.GetBool("validatemethodstrictly", true);
            this.UseAuthentication = settings.GetBool("authenticate", false);
            this.MimeType = settings.GetString("mimetype", "application/x-www-form-urlencoded");
            this.ProxyServerVariable = settings.GetString("proxyvar", "");
            if (settings.Has("bearer"))
            {
                this.UseBearer = true;
                this.BearerToken = settings.GetString("bearer");
            }
            if (settings.Has("bearer_var"))
            {
                this.UseBearerVar = true;
                this.BearerVar = settings.GetString("bearer_var");
            }

            foreach (var item in settings.Dictionary)
			{
				if (item.Key.EndsWith("_override")) {
					HeaderMapping[item.Value.ToString()] = item.Key.Substring(0, item.Key.Length - 9);
				}
			}

			if (Array.IndexOf (ValidMethods, this.Method) < 0) {
				string message = "Method should be \"OPTIONS\", \"GET\", \"HEAD\", " +
					"\"POST\", \"PUT\", \"DELETE\", \"TRACE\" or \"CONNECT\"";
				if (this.IsMethodValidatedStrictly) {
					throw new ArgumentException (message);
				} else {
					Secretary.Report (5, message);
				}
			}
		}

		/// <summary>
		/// Gets the Content Type Request Header for this HTTP call
		/// </summary>
		/// <returns>The content type.</returns>
		string GetContentType ()
		{
			return string.Format ("{0}; charset={1}", this.MimeType, this.UrlEncoding.WebName);
		}

		protected override void HandleBranchChanged (object sender, ItemChangedEventArgs<Service> e)
		{
			base.HandleBranchChanged (sender, e);

			if (e.Name == "uri") this.URI = e.NewValue;
			else if (e.Name == "body") this.Body = e.NewValue;
		}

		/// <summary>
		/// Attempts to assemple requesst URI
		/// </summary>
		/// <returns><c>true</c>, if the attempt was successful.</returns>
		/// <param name="parameters">Parameters.</param>
		/// <param name="uri">Output URI.</param>
		protected bool TryProduceURI (IInteraction parameters, out string uri)
		{	
			bool successful = true;

			if (this.URI == null) {
				uri = this.DefaultURI;
			} else {
				StringComposeInteraction uriComposer = new StringComposeInteraction (parameters, this.UrlEncoding);

				successful &= this.URI.TryProcess (uriComposer);

				uri = uriComposer.ToString ();
			}

			return successful;
		}

        /// <summary>
        /// Produces the request.
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="uriString">URI string.</param>
        HttpWebRequest ProduceRequest(IInteraction parameters, string uriString)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uriString);
            request.AllowAutoRedirect = Settings.GetBool("allowautoredirect", true);
            if (this.ProxyServerVariable.Length > 0) {
                var proxyServer = Fallback<String>.From(parameters, this.ProxyServerVariable);
                if ((proxyServer != null) && (proxyServer.Length > 0)) {
                    request.Proxy = new WebProxy(proxyServer);
                }
            }
            request.Method = this.Method;
            request.ContentType = GetContentType();
            request.Expect = "200";

            foreach (var header in this.HeaderMapping.Dictionary)
            {
                string resultstr;
                if (parameters.TryGetFallbackString(header.Value.ToString(), out resultstr)) {
                    request.Headers[header.Key] = resultstr;
                }
            }

            if (this.UseAuthentication) 
                request.Credentials = Credentials.Recover(parameters);
            else if (this.UseBearerVar) 
                request.Headers["Authorization"] = "Bearer " + Fallback<string>.From(parameters, BearerVar);
            else if (this.UseBearer) 
                request.Headers["Authorization"] = "Bearer " + this.BearerToken;

			if (this.Body != null) {
				var stream = request.GetRequestStream ();
				var bodyBuilder = new SimpleOutgoingInteraction (stream, parameters);

				if (!this.Body.TryProcess (bodyBuilder)) {
					throw new Exception ("Failed to compose body");
				}

				bodyBuilder.Done ();
			}

			return request;
		}

		protected override bool Process (IInteraction parameters)
		{
			bool successful = true;
			string uriString;

			if (successful &= TryProduceURI (parameters, out uriString)) {
				HttpWebRequest request = ProduceRequest (parameters, uriString);

				HttpWebResponse response = null;
				try {
					response = (HttpWebResponse)request.GetResponse ();
					int statusInt = (int)response.StatusCode;

					HTTPResponseInteraction responseInteraction;
					responseInteraction = new HTTPResponseInteraction (request, response, parameters);

					successful = (statusInt >= 200) && (statusInt < 300);
					successful = successful && Successful.TryProcess (responseInteraction);
					successful = successful || Failure.TryProcess (responseInteraction);
				} catch (WebException ex) {
					var exceptionInteraction = new HTTPResponseInteraction(request, ex.Response, parameters);

					successful = Failure.TryProcess(exceptionInteraction);
				} finally {
					if (response != null) response.Dispose();
				}
			}

			return successful;
		}
	}
}

