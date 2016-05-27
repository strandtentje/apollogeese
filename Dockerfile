FROM debian:stretch

RUN apt-get update && apt-get install -y \
        bash \
        ca-certificates \
        mono-complete \
        htop \
        screen \
        wget \
        tar \
        unzip

ENV RELEASE_ROOT https://github.com/strandtentje/apollogeese/releases/download
ENV AG_TAG v17
ENV AG_ZIP Release.17.zip
ENV AG_INSTALL_PATH /opt/ag

RUN mkdir -p /tmp
    && mkdir -p $AG_INSTALL_PATH
    && curl -fSL "$RELEASE_ROOT/$AG_TAG/$AG_ZIP" -o /tmp/ag.zip
    && unzip /tmp/ag.zip -d $AG_INSTALL_PATH
    && echo "#!/bin/bash" >> /usr/bin/ag
    && echo "mono $AG_INSTALL_PATH/agrun.exe -pfb true -pfc false -l -c $@" >> /usr/bin/ag
    && chmod +x /usr/bin/ag


##<autogenerated>##
CMD ["php", "-a"]
##</autogenerated>##
