# CEN315 Fitness Service

# 🏋️ Fitness Service API (CEN315 Term Project)
![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)

## 📖 Executive Summary
The **Fitness Service API** is a robust RESTful backend built with **.NET 8** designed to manage a modern fitness center. It handles member registrations, class scheduling, and complex reservation logic with dynamic pricing.

This project was developed using **Test-Driven Development (TDD)** and **Clean Architecture** principles, ensuring high maintainability and testability. It features a fully automated CI/CD pipeline and has been hardened against concurrency issues and security vulnerabilities.

---

## 🚀 Key Features

* **Dynamic Pricing Engine:** Calculates reservation costs based on Membership Tier (Standard/Premium), Peak Hours (18:00-22:00), and Capacity Surges (>80% occupancy).
* **Concurrency Control:** Atomic transactions prevent overbooking when multiple users attempt to book the last spot simultaneously.
* **Clean Architecture:** Strict separation of concerns between the API Layer, Business Logic (Services), and Domain Entities.
* **Resilience:** Dockerized deployment with self-healing capabilities (`restart: always` policy).
* **Security:** Implementing ZAP.

---

## 🛠️ Tech Stack

| Component | Technology |
| :--- | :--- |
| **Framework** | .NET 8 (C#) |
| **Architecture** | RESTful API, Clean Architecture |
| **Testing** | xUnit, Moq, Coverlet, Stryker.NET (Mutation Testing) |
| **Integration** | Postman |
| **Documentation** | Swagger / OpenAPI |

---

## 🧪 Testing Strategy

This project employs a "Shift-Left" testing strategy with >80% code coverage.

### 1. Unit Testing (TDD)
We utilize **xUnit** and **Moq** to test business logic in isolation.
* **Run Tests:** `dotnet test`
* **Key Focus:** Pricing logic, Capacity validation, Null checks.

### 2. Integration Testing
End-to-End workflows are validated using **Postman**.
* **Collection:** `Fitness_Collection.json`
* **Scenarios:** Member Creation -> Class Scheduling -> Reservation -> Cancellation.

### 3. Advanced Testing
* **Combinatorial Testing:** Used Pairwise testing (PICT) to optimize pricing scenarios (Peak/Off-Peak vs. Standard/Premium).
* **Mutation Testing:** Used Stryker.NET to identify weak tests and improve assertion quality.
