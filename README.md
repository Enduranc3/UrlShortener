# URL Shortener

A full-stack URL shortening application built with React frontend and .NET backend.

## Project Structure

- `url-shortener-frontend/` - React frontend application
- `UrlShortenerApi/` - .NET backend API
- `UrlShortenerApiTests/` - Unit tests for the backend API

## Getting Started

### Backend (UrlShortenerApi)

1. Navigate to the API directory:

```sh
cd UrlShortenerApi
```

2. Create .env file in the root of the API directory and add the following environment variables:

```sh
CONNECTION_STRING=YOUR_CONNECTION_STRING
SECRET_KEY=YOUR_SECRET_KEY
```

3. Run the migrations:

```sh
dotnet-ef database update
```

4. Run the API:

```sh
dotnet run
```

### Frontend (url-shortener-frontend)

1. Navigate to the frontend directory:

```sh
cd url-shortener-frontend
```

2. Install dependencies:

```sh
npm install
```

3. Run the frontend:

```sh
npm start
```

## Features

Shorten long URLs into compact, easy-to-share links
View list of shortened URLs
Track usage statistics
Responsive web interface

## Tech Stack

### Backend

- .NET 9.0
- C#
- Entity Framework Core
- PostgreSQL

### Frontend

- React
- TypeScript

## Testing

To run the backend tests:

```sh
cd UrlShortenerApiTests
dotnet test
```

## Project Overview
![](https://github.com/Enduranc3/UrlShortener/blob/main/demo.gif)
