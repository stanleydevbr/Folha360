FROM pgvector/pgvector:pg16

# Instalar locale pt_BR.UTF-8
RUN apt-get update && \
    apt-get install -y locales && \
    echo "pt_BR.UTF-8 UTF-8" >> /etc/locale.gen && \
    locale-gen pt_BR.UTF-8 && \
    update-locale LANG=pt_BR.UTF-8 && \
    rm -rf /var/lib/apt/lists/*

ENV LANG=pt_BR.UTF-8
ENV LANGUAGE=pt_BR:en
