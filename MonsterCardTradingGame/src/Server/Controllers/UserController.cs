﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using MonsterCardTradingGame.Server;
using MonsterCardTradingGame.Services;

namespace MonsterCardTradingGame.Controllers
{
    public class UserController
    {
        // Handles user registration
        public static HTTPResponse HandleRegistration(HTTPRequest request)
        {
            try
            {
                // Parse the body JSON string into a dictionary
                var body = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);

                if (body == null || !body.ContainsKey("username") || !body.ContainsKey("password"))
                {
                    return new HTTPResponse(400, "{\"error\": \"Username and password are required\"}");
                }

                string username = body["username"];
                string password = body["password"];

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return new HTTPResponse(400, "{\"error\": \"Username and password cannot be empty\"}");
                }

                bool isRegistered = UserService.RegisterUser(username, password);

                if (isRegistered)
                {
                    return new HTTPResponse(201, "{\"message\": \"User registered successfully\"}");
                }
                else
                {
                    return new HTTPResponse(409, "{\"error\": \"User already exists\"}");
                }
            }
            catch (JsonException)
            {
                return new HTTPResponse(400, "{\"error\": \"Invalid JSON format\"}");
            }
        }

        // Handles user login (authentication)
        public static HTTPResponse HandleLogin(HTTPRequest request)
        {
            try
            {
                // Parse the body JSON string into a dictionary
                var body = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);

                if (body == null || !body.ContainsKey("username") || !body.ContainsKey("password"))
                {
                    return new HTTPResponse(400, "{\"error\": \"Username and password are required\"}");
                }

                string username = body["username"];
                string password = body["password"];

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return new HTTPResponse(400, "{\"error\": \"Username and password cannot be empty\"}");
                }

                // Authenticate user
                if (UserService.Authenticate(username, password))
                {
                    return new HTTPResponse(200, "{\"message\": \"Login successful\"}");
                }
                else
                {
                    return new HTTPResponse(401, "{\"error\": \"Invalid username or password\"}");
                }
            }
            catch (JsonException)
            {
                return new HTTPResponse(400, "{\"error\": \"Invalid JSON format\"}");
            }
        }
    }
}
