# 🎟 High-Volume Ticket Booking System

A sample **.NET 8** solution that demonstrates how to design, build, and test a **high-concurrency ticket booking system**.  
The system implements **optimistic concurrency control**, **background queue processing**, and **Redis caching** to efficiently handle a large number of booking requests while preventing double-booking of seats.

---

## 📌 Features
- **EF Core** with SQL Server for persistence
- **Optimistic Concurrency** to handle race conditions when booking seats
- **Custom Exception Handling & Centralized Logging**
- **Background Worker Queue** for async booking processing
- **Redis Cache** for seat availability lookups
- **Unit Tests** using NUnit and Moq

---


## 📂 Technologies Used
- **.NET 8**
- **Entity Framework Core**
- **SQL Server** (can be local or containerized)
- **Redis** for caching (via `Microsoft.Extensions.Caching.StackExchangeRedis`)
- **BackgroundService** for queued processing
- **Serilog** for structured logging
- **NUnit & Moq** for testing

---

## 🚀 Getting Started

### 1️⃣ Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (or Docker container)
- [Redis](https://redis.io/) (local install or Docker container)
- [Docker](https://www.docker.com/) (optional, for containerized services)

---

### 2️⃣ Setup

1. **Clone the repository**
```bash
git clone https://github.com/bigboybamo/MovieTicketBookingSystem.git
cd TicketBooking

