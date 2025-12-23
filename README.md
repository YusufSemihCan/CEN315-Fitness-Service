# CEN315 Fitness Service

# 🏋️ Fitness Service API (CEN315 Term Project)

![Build Status](https://img.shields.io/github/actions/workflow/status/YOUR_GITHUB_USERNAME/YOUR_REPO_NAME/dotnet.yml?branch=main)
![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)
![Docker](https://img.shields.io/badge/Docker-Ready-blue)
![License](https://img.shields.io/badge/License-MIT-green)

## 📖 Executive Summary
The **Fitness Service API** is a robust RESTful backend built with **.NET 8** designed to manage a modern fitness center. It handles member registrations, class scheduling, and complex reservation logic with dynamic pricing.

This project was developed using **Test-Driven Development (TDD)** and **Clean Architecture** principles, ensuring high maintainability and testability. It features a fully automated CI/CD pipeline and has been hardened against concurrency issues and security vulnerabilities.

---

## 🚀 Key Features

* **Dynamic Pricing Engine:** Calculates reservation costs based on Membership Tier (Standard/Premium), Peak Hours (18:00-22:00), and Capacity Surges (>80% occupancy).
* **Concurrency Control:** Atomic transactions prevent overbooking when multiple users attempt to book the last spot simultaneously.
* **Clean Architecture:** Strict separation of concerns between the API Layer, Business Logic (Services), and Domain Entities.
* **Resilience:** Dockerized deployment with self-healing capabilities (`restart: always` policy).
* **Security:** Custom middleware implementing `X-Content-Type-Options` and `X-Frame-Options` headers.

---

## 🛠️ Tech Stack

| Component | Technology |
| :--- | :--- |
| **Framework** | .NET 8 (C#) |
| **Architecture** | RESTful API, Clean Architecture |
| **Testing** | xUnit, Moq, Coverlet, Stryker.NET (Mutation Testing) |
| **Integration** | Postman, Newman |
| **Containerization** | Docker, Docker Compose |
| **CI/CD** | GitHub Actions |
| **Documentation** | Swagger / OpenAPI |

---

## ⚙️ Getting Started

### Prerequisites
* .NET 8 SDK
* Docker Desktop (Optional, for container mode)

### Option 1: Run Locally
1.  Clone the repository:
    ```bash
    git clone [https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git](https://github.com/YOUR_USERNAME/YOUR_REPO_NAME.git)
    ```
2.  Navigate to the project folder:
    ```bash
    cd Fitness_Service_API
    ```
3.  Run the application:
    ```bash
    dotnet run
    ```
4.  Open your browser to the Swagger UI:
    * `http://localhost:5000/swagger` (or port shown in terminal)

### Option 2: Run with Docker
1.  Build and run the container:
    ```bash
    docker build -t fitness-service .
    docker run -p 8080:8080 fitness-service
    ```
2.  Access the API at `http://localhost:8080/swagger`.

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

---

## 📂 Project Structure

```text
├── .github/workflows/    # CI/CD Pipeline (dotnet.yml)
├── Fitness_Service_API/  # Main Application Code
│   ├── Controllers/      # API Endpoints
│   ├── Services/         # Business Logic (Pricing, Reservations)
│   └── Entities/         # Domain Models
├── Fitness_Service_UnitTest/ # xUnit Test Suite
├── Dockerfile            # Container Configuration
└── README.md             # Project Documentation
