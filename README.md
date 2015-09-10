ApolloGeese/Clon
================

ApolloGeese represents a design philosophy, a collection of essential web-application back-end modules and a simple language to provide a means of loosely coupling them.

This is a proof of concept implementation in Mono.

# Compiling/Running

Acquiring binaries can be done by

1. Downloading them from the releases-section on github (preferred)
2. Using an IDE such as Visual Studio (Express edition 2012 and maybe even 2008 should suffice)
3. Using xbuild or msbuild from the command line

## Using mono

	xbuild /p:Configuration=Release ApolloGeese.sln  /p:OutputPath=/an/output/folder/of/choice

## Using .Net

Probably same as above, but with msbuild. Recommended route on Windows.

# Distribution

Please distribute license files along with the binaries. It is somewhat recommended to supply a default apollogeese.conf that does nothing, along with the binaries. I.e:

	{
		plugins = [f"CoreTypes.dll"];
		instances = {
			none = StubService();
		};
	};
