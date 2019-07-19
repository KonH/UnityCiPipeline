FROM ubuntu:18.04

ENV DEBIAN_FRONTEND noninteractive
ENV DEBCONF_NONINTERACTIVE_SEEN true
ENV APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE=1
ENV LANGUAGE C.UTF-8
ENV LANG C.UTF-8
ENV LC_ALL C.UTF-8
ENV LC_CTYPE C.UTF-8
ENV LC_MESSAGES C.UTF-8

# Main dependencies
RUN apt-get update -qq
RUN apt-get install -qq -y \
    gconf-service \
    lib32gcc1 \
    lib32stdc++6 \
    libasound2 \
    libarchive13 \
    libc6 \
    libc6-i386 \
    libcairo2 \
    libcap2 \
    libcups2 \
    libdbus-1-3 \
    libexpat1 \
    libfontconfig1 \
    libfreetype6 \
    libgcc1 \
    libgconf-2-4 \
    libgdk-pixbuf2.0-0 \
    libgl1-mesa-glx \
    libglib2.0-0 \
    libglu1-mesa \
    libgtk2.0-0 \
    libgtk3.0 \
    libnotify4 \
    libnspr4 \
    libnss3 \
    libpango1.0-0 \
    libsoup2.4-1 \
    libstdc++6 \
    libx11-6 \
    libxcomposite1 \
    libxcursor1 \
    libxdamage1 \
    libxext6 \
    libxfixes3 \
    libxi6 \
    libxrandr2 \
    libxrender1 \
    libxtst6 \
    libunwind-dev \
    zlib1g \
    pulseaudio \
    debconf \
    npm \
    xdg-utils \
    lsb-release \
    libpq5 \
    xvfb \
    wget \
    ruby-dev \
    git \
    curl \
    sudo
RUN apt-get clean
RUN rm -rf /var/lib/apt/lists/*

# Update mono
RUN apt-get install gnupg ca-certificates
RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
RUN echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic/snapshots/5.20 main" | tee /etc/apt/sources.list.d/mono-official-stable.list
RUN apt-get update -qq
RUN apt-get install -qq -y mono-devel
RUN apt-get clean
RUN rm -rf /var/lib/apt/lists/*

# U3D for further unity install
# Oficial way
# RUN gem install u3d -v '1.1.5' && gem list
# Trying to fix available issue
RUN git clone https://github.com/DragonBox/u3d.git \
    && cd u3d \
    && git log -1 \
    && chmod a+x build.sh \
    && ./build.sh
RUN u3d -v

# Butler for itch.io upload
RUN wget -O butler.zip https://broth.itch.ovh/butler/linux-amd64/LATEST/archive/default \
    && unzip butler.zip -d /usr/bin \
    && chmod +x /usr/bin/butler \
    && butler -V

ADD CACerts.pem /root/.local/share/unity3d/Certificates/

ADD activate.sh /
ADD build.sh /
ADD clone.sh /
ADD install.sh /
