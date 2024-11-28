# WeatherFetcher
Hey hey! Welcome to my best effort to tackle the JB HI-FI tech challenge

## Setup
I instantiated the codebase and the included projects using dotnet commands so you can run the code easily using either your IDE of choice (I used Rider as Visual studio no longer supports MacOs) or dotnet commands directly!
### Commands
#### Build from base folder
`dotnet build`

#### Running WeatherFetcherApi
`cd WeatherFetcherApi`
`dotnet run`

#### Running WeatherFetcherApiTests
`cd WeatherFetcherApiTests`
`dotnet test`

#### Running WeatherFetcherWeb
`cd WeatherFetcherWeb`
`dotnet run`

#### Running WeatherFetcherWebTests
`cd WeatherFetcherWebTests`
`dotnet run`

## Choices
### Minimal API
I have never used a minimal API before and I thought it would be a good learning opportunity for me to tackle. I also wanted the api to be lightweight to show a focus on performance

### Razor Pages
I also have never used Razor pages (2 new techs in one challenge!) but decided I wanted to use something from the dotnet wheelhouse as it seemed like a good exercise

### API key / Rate limiting middleware
I was debating spinning another seperate project for managing the rate limiting/API key handling but settled on using the AspNetCoreRateLimit package and writing a simple API key middleware. I chose to do this because I wanted to utilise existing packages for an opportunity to learn and to keep things simpler to maintain or extend later

## What to do next / Improve
### Frontend
Honestly if I had more time I would have most likely swapped to a react app for the frontend just because I could showcase my experience better

### Logging
I added some logging where I thought it was relevant but I would have done a few more passes to add more if I had the time

### Extending my API handler
During this tech challenge I actually had a working API key handler that extended AuthenticationSchemeOptions but I couldnt seem to get my tests working for it. I ran into issues mocking the config files it needed (I spent a while on this but no dice) so I pivoted to writing the ApiKeyMiddleware 

## Folder Structure
I know that GitHub UI can be cumbersome when checking folder structure so heres my current folder structure (Build files excluded)
The backend and frontend codebases are seperately instantiated so they can be run and maintained independently. I have followed best practice in folder naming conventions to help with readability and to ensure correct seperation of concern. The test projects mirror the folder structure of the project they are testing.

<img width="339" alt="Screenshot 2024-11-28 at 1 30 33 pm" src="https://github.com/user-attachments/assets/3faba03d-4f36-4516-a417-71dd85281a6a">

## Thanks for reviewing my code!!
Hope to hear back soon!
