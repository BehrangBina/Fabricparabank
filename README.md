# FabricParaBank Test Automation Framework

A comprehensive test automation framework for testing the ParaBank web application using .NET, Playwright, and Reqnroll (SpecFlow successor) with behavior-driven development (BDD) approach.

## Overview

This framework provides end-to-end automated testing capabilities for the ParaBank banking application, covering user registration, login, account management, fund transfers, and bill payments. It uses the Page Object Model pattern and follows BDD principles for maintainable and readable test scenarios.

## Technology Stack

- **.NET 9.0** - Core framework
- **Playwright** - Browser automation
- **Reqnroll** - BDD testing framework (SpecFlow successor)
- **xUnit** - Test runner
- **FluentAssertions** - Assertion library
- **Bogus** - Test data generation
- **C#** - Programming language

## Project Structure

```

FabricParaBank/
├── Features/                    # BDD feature files
│   └── AccountManagement.feature
├── StepDefinitions/            # Step implementations
│   ├── AccountManagementStepDefinitions.cs
│   └── ApiStepDefinitions.cs
├── Pages/                      # Page Object Model classes
│   ├── LoginPage.cs
│   ├── RegistrationPage.cs
│   └── AccountPage.cs
├── PageObjects/               # Page locators and base classes
│   ├── BasePage.cs
│   └── Locators/
├── Model/                     # Data models
│   ├── TestUser.cs
│   └── PayeeUser.cs
├── Util/                      # Utilities and helpers
│   ├── PlaywrightTestBase.cs
│   ├── TestDataHelper.cs
│   └── ConfigManager/
├── Builders/                  # Builder pattern implementations
│   └── TestUserBuilder.cs
├── Constants/                 # Test constants
│   └── TestConstants.cs
├── Enums/                     # Enumeration types
│   ├── AccountType.cs
│   └── TransactionType.cs
└── Hooks/                     # Test hooks and setup
    └── PlaywrightScenarioHooks.cs
```

## Core Components

### Page Object Model

The framework implements the Page Object Model pattern for maintainable UI automation:

- **BasePage**: Common page functionality and utilities
- **LoginPage**: Handles user authentication
- **RegistrationPage**: Manages user registration
- **AccountPage**: Banking operations (transfers, bill payments, account creation)

### Configuration Management

Centralized configuration through `appsettings.json`:

```json
{
  "TestSettings": {
    "BaseUrl": "https://parabank.parasoft.com/",
    "Browser": {
      "Type": "chromium",
      "Headless": false,
      "SlowMotion": true,
      "SlowMotionDelay": 100,
      "Viewport": {
        "Width": 1920,
        "Height": 1080
      }
    }
  }
}
```

### Test Data Generation

Uses Bogus library for generating realistic test data:

- **TestUser**: Complete user profiles with personal information
- **PayeeUser**: Payee information for bill payments
- **TestDataHelper**: Centralized data generation utilities

### BDD Test Scenarios

Feature files written in Gherkin syntax covering:

- User registration and login
- Account creation (Savings accounts)
- Fund transfers between accounts
- Bill payment functionality
- Account overview validation
- Navigation menu testing

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code
- Git

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd FabricParaBank
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Install Playwright browsers:
```bash
dotnet run playwright install
```

### Running Tests

Execute all tests:
```bash
dotnet test
```

Run specific test scenarios:
```bash
dotnet test --filter "TestCategory=Smoke"
```

Run with specific browser:
```bash
dotnet test -- Browser.Type=firefox
```

### Configuration

Modify `appsettings.json` to customize:

- **BaseUrl**: Target application URL
- **Browser Type**: chromium, firefox, or webkit
- **Headless Mode**: true/false for headless execution
- **Viewport Size**: Browser window dimensions
- **Slow Motion**: Debugging aid with configurable delay

## Test Scenarios

### End-to-End Banking Workflow

The main scenario covers a complete banking workflow:

1. **User Registration**: Create new user with unique credentials
2. **Login**: Authenticate with registered credentials  
3. **Navigation Validation**: Verify all menu options are available
4. **Account Creation**: Create a new Savings account
5. **Fund Transfer**: Transfer money between accounts
6. **Bill Payment**: Pay bills using the new account
7. **Validation**: Verify all transactions completed successfully

### API Testing

Includes API testing capabilities for:
- Transaction searches
- Account validation
- Response verification

## Best Practices

### Code Organization

- **Separation of Concerns**: Clear separation between page objects, test data, and test logic
- **DRY Principle**: Reusable components and utilities
- **SOLID Principles**: Well-structured, maintainable code architecture

### Test Design

- **Page Object Model**: Encapsulated page interactions
- **BDD Approach**: Business-readable test scenarios
- **Data-Driven Testing**: Parameterized test scenarios
- **Robust Locators**: Stable element identification strategies

### Error Handling

- **Fluent Assertions**: Clear, readable test assertions
- **Comprehensive Logging**: Detailed test execution logs
- **Wait Strategies**: Proper synchronization handling

## Maintenance

### Adding New Tests

1. Create feature file in `Features/` directory
2. Implement step definitions in `StepDefinitions/`
3. Add page objects in `Pages/` as needed
4. Update models in `Model/` for new data structures

### Updating Locators

- Centralize locators in `PageObjects/Locators/`
- Use stable locator strategies (data-testid preferred)
- Implement BasePage for common functionality

### Configuration Updates

- Modify `appsettings.json` for environment changes
- Update `ConfigLoader.cs` for new configuration options
- Maintain backward compatibility

## Troubleshooting

### Common Issues

1. **Browser Installation**: Run `dotnet run playwright install`
2. **Test Failures**: Check application availability and network connectivity
3. **Locator Issues**: Verify element selectors in browser developer tools
4. **Configuration**: Validate `appsettings.json` syntax and values
