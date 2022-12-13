# Backend service for a calendar app.

This is the backend project for the Udemy course: "Udemy-React de cero a experto", which I used to get up to speed with react fast.
The course provides a sample backend in nodejs, I decided to deploy it using aspnet core 7. These project allowed me to learn how the new minimal APIs work, as well as experiment with asp.net deployment on the [render.com](https://render.com/) platform via docker.


This service uses a mongo atlas database for storage.

The mongo connection string should be supplied in the `src/appsettings.Development.json` file (since that's in the gitignore path), these are the required settings for the development file:

```json
{
    "CalendarDBSettings": {
        "ConnectionString": "MongoDB Connection String here"
    }
}
```

To have it run in docker, first build the image:

`docker build -t calendar-backend-net -f dockerfile .`

the run the container (replace mongo connection string):

`docker run -it --rm -p 5000:80 --name calendar-backend-net calendar-backend-net -e "CalendarDBSettings:ConnectionString=<MongoDBConnectionString"`