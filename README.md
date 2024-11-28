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

## Assumptions

## What to do next / Improve

## Thanks for reviewing my code!!
Hope to hear back soon!

## Folder Structure
I know that GitHub UI can be cumbersome when checking folder structure so heres my current folder structure (Build files excluded)
The backend and frontend codebases are seperately instantiated so they can be run and maintained independently. I have followed best practice in folder naming conventions to help with readability and to ensure correct seperation of concern. The test projects mirror the folder structure of the project they are testing.
<img width="339" alt="Screenshot 2024-11-28 at 1 30 33 pm" src="https://github.com/user-attachments/assets/c50c05a6-5ab9-4d33-8f02-e56feea18a93">
