FROM postgres:latest

COPY seed.sql /docker-entrypoint-initdb.d/

ENV POSTGRES_USER=myuser
ENV POSTGRES_PASSWORD=mypassword
ENV POSTGRES_DB=mydb

EXPOSE 5432

# docker exec -it 4dab476f0986 psql -h localhost -U myuser mydb

