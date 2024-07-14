## Answers to technical questions

## How long did you spend on the coding test? What would you add to your solution if you had more time? If you didn't spend much time on the coding test then use this as an opportunity to explain what you would add.

I spent approximately 2 hours on the coding test. If I had more time, I would focus on the following improvements:

- Enhanced Validation: Implement more sophisticated email validation using regular expressions to ensure that the email format is strictly adhered to.
  I tried implementing, but could not complete.SO reverted back to simplest form.
- Logging: Add comprehensive logging to help with debugging and tracking the application's behavior.
- Error Handling: Improve error handling by introducing custom exception classes and handling edge cases more gracefully.
- Refactoring: Further refactor the code to enhance readability and maintainability, such as extracting more methods and possibly creating more classes to handle specific responsibilities.
  LLike for example the connectionString in ClientRepository and again in the UserDataAccess class.Since instruction was not to touch the class, did not bother about using the separate connectionString class.
- Performance: Analyze and optimize the performance of the code, particularly focusing on database access and network calls.
- Documentation: Add XML comments and detailed documentation to the code to make it easier for other developers to understand and maintain.

## What was the most useful feature that was added to the latest version of C#? Please include a snippet of code that shows how you've used it.

One of the most useful features in the latest version of C# (C# 10) is the `record` type Here's a snippet of how I've used it in this project:

public record User
{
public string Firstname { get; init; }
public string Surname { get; init; }
public DateTime DateOfBirth { get; init; }
public string EmailAddress { get; init; }
public bool HasCreditLimit { get; set; }
public int CreditLimit { get; set; }
public Client Client { get; init; }
}

How would you track down an issue with this in production, assuming this API would be deployed as an app service in Azure.
To track down an issue in production with this API deployed as an Azure App Service, I would follow these steps:

Monitoring and Logging: Ensure that comprehensive logging is implemented using a logging framework like Serilog or NLog. Logs should be configured to be written to Azure Application Insights or Azure Monitor for centralized monitoring and alerting.
Application Insights: Utilize Azure Application Insights to monitor the performance, track dependencies, and log custom events and metrics.
Alerts : Set up alerts to notify the development team of any anomalies or failures.
Diagnostics Logs: Enable diagnostics logging for the Azure App Service to capture detailed logs, including HTTP request and response logs, error logs, and performance logs.
Health Checks: Implement health check endpoints and configure Azure App Service to perform regular health checks to ensure the application is running as expected.
Performance Monitoring: Use Application Insights to analyze performance metrics, such as response times, request rates, and failure rates. Identify any bottlenecks or performance issues.
Error Analysis: Analyze the logs and telemetry data to identify the root cause of the issue. Look for patterns, exceptions, and error messages that provide clues about the problem.
Remote Debugging: If necessary, use the remote debugging feature in Visual Studio to connect to the running instance of the Azure App Service and debug the application in real-time.
Feedback Loop & Colloboration: Once the issue is identified and fixed, ensure that the feedback loop is in place to update the monitoring and logging configurations to prevent similar issues in the future.
Collaborate with the development and operations teams to gather insights and work together on resolving the issue. Utilize tools like Azure DevOps for tracking and managing the resolution process.
By following these steps, I can effectively track down and resolve issues in the production environment, ensuring the stability and reliability of the API.

## Git Repo :

https://github.com/NithyaMohan/Refactoring
