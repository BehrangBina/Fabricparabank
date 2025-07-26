Feature: Customer Onboarding and Banking Operations
As a new user
I want to register, log in, and perform basic banking tasks
So that I can use the core features of the Para Bank application

    Background: Registration of a unique user
        # Registration
        Given I navigate to the Para Bank application
        When I register a new user with a unique username
        Then I can see welcome message on the screen
        Then I logout from the system

    Scenario: End-to-end banking workflow for a newly registered user
        # Login
        When I login using the newly registered user credentials
        # Validating navigation
        Then I can validate global navigation menu for the logged in user
          | Navigation Links    |
          | Open New Account    |
          | Accounts Overview   |
          | Transfer Funds      |
          | Bill Pay            |
          | Find Transactions   |
          | Update Contact Info |
          | Request Loan        |
          | Log Out             |
        # Create a saving account, transfer amount, pay bill
        When I create a new account of type "Open New Account"
        Then I create a "Savings" account and validate it
        And I click on "Transfer Funds"      
        And I transfer "100" to the created account
        Then Transfer has been successfully completed
        And I click on "Bill Pay"
        When I pay a bill using the new account      
        And I click on "Accounts Overview"
        Then the payment should be processed and balance updated