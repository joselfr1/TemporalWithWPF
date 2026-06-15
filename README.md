# Temporalio Hello World - WPF Edition

A learning-focused pet project demonstrating **WPF** desktop application development combined with **Temporal.io** for distributed workflow orchestration.

## 📋 Project Description

This project is a hands-on practice implementation built to explore two key areas:
1. **WPF (Windows Presentation Foundation)** - Building modern desktop UI with data binding
2. **Temporal.io** - Learning distributed workflow orchestration with the Temporal C# SDK

The application provides a simple UI where users enter a name, trigger a Temporal workflow, and receive a greeting response orchestrated through Temporal's distributed system.

---

## 📁 Project Structure & Organization

### Complete Folder Tree

```
TemporalioHelloWorld/
│
├── 📂 HelloWinUI/                          [WPF Desktop Application - Presentation Layer]
│   ├── App.xaml
│   ├── App.xaml.cs                         (DI Container Setup & Startup)
│   ├── MainWindow.xaml
│   ├── MainWindow.xaml.cs                  (UI Handler)
│   ├── HelloWinUI.csproj                   (Target: net10.0-windows, UseWPF: true)
│   ├── appsettings.json                    (Temporal Server Config)
│   └── 📂 ViewModels/
│       └── SayHelloViewModel.cs            (MVVM ViewModel - INotifyPropertyChanged)
│
├── 📂 Model/                               [Shared Abstractions & DTOs]
│   ├── Model.csproj
│   ├── 📂 Dtos/
│   │   └── ExecutionWorkflowClient.cs      (Configuration POCO)
│   ├── 📂 Ports/
│   │   └── ISayHelloWorkflow.cs            (Workflow Interface - Port Pattern)
│   └── 📂 Configurations/
│       └── Configurations.cs               (Base Config Class)
│
├── 📂 Workflow/                            [Temporal Workflow Definitions]
│   ├── Workflow.csproj
│   └── 📂 Workflows/
│       └── SayHelloWorkflow.cs             (ISayHelloWorkflow Implementation)
│
├── 📂 Services/                            [Business Logic & Activities]
│   ├── Services.csproj
│   └── 📂 Activities/
│       └── GreetingActivity.cs             (Temporal Activity)
│
├── 📂 Worker/                              [Temporal Worker Host - Background Service]
│   ├── Worker.csproj                       (OutputType: Exe)
│   ├── Program.cs                          (Worker Startup & Registration)
│   └── appsettings.json                    (Worker Config)
│
├── 📂 Client/                              [Temporal Client - Console App]
│   ├── Client.csproj                       (OutputType: Exe)
│   ├── Program.cs                          (Workflow Execution Client)
│   └── appsettings.json
│
├── 📂 Utils/                               [Cross-Cutting Concerns - DI & Configuration]
│   ├── Utils.csproj
│   ├── ConfigurationBuilder.cs             (Configuration Loading)
│   └── 📂 TemporalSetup/
│       └── TemporalClientFactory.cs        (Temporal Client Initialization)
│
├── 📂 WpfClientTests/                      [WPF Presentation Tests]
│   ├── WpfClientTests.csproj               (Target: net10.0-windows)
│   └── SayHelloViewModelTests.cs           (ViewModel Unit Tests - Xunit + Moq)
│
├── 📂 WorkflowTests/                       [Workflow Logic Tests]
│   ├── WorkflowTests.csproj
│   └── SayHelloWorkflowTests.cs            (Workflow Tests)
│
├── 📂 ServicesTests/                       [Services & Activities Tests]
│   ├── ServicesTests.csproj
│   └── GreetingActivityTests.cs            (Activity Tests)
│
├── TemporalioHelloWorld.sln                [Solution File]
└── README.md                               [This File]
```

### Key Files by Layer

| File | Layer | Purpose |
|------|-------|---------|
| `HelloWinUI/App.xaml.cs` | Presentation | Bootstraps DI container, registers services, shows MainWindow |
| `HelloWinUI/MainWindow.xaml.cs` | Presentation | XAML UI handler, delegates workflow execution to ViewModel |
| `HelloWinUI/ViewModels/SayHelloViewModel.cs` | Presentation | MVVM ViewModel, manages UI state, orchestrates workflow calls |
| `Model/Ports/ISayHelloWorkflow.cs` | Shared | Workflow interface contract (Temporal [Workflow] attribute) |
| `Workflow/Workflows/SayHelloWorkflow.cs` | Workflow | Implements ISayHelloWorkflow, orchestrates activities |
| `Services/Activities/GreetingActivity.cs` | Services | Temporal [Activity], performs greeting business logic |
| `Worker/Program.cs` | Worker | Registers workflows & activities, hosts worker for Temporal Server |
| `Client/Program.cs` | Client | Creates ITemporalClient, executes workflows from CLI |
| `Utils/ConfigurationBuilder.cs` | Shared | Loads appsettings.json with Microsoft.Extensions.Configuration |
| `Utils/TemporalSetup/TemporalClientFactory.cs` | Shared | Factory pattern for ITemporalClient creation |

---

## 🏛️ Layered Architecture

This project uses a **6-layer architecture** with clear separation of concerns. Each layer has specific responsibilities and communicates through well-defined interfaces.

### Layer 1: **Presentation Layer** (HelloWinUI)
**Purpose:** User Interface and User Interaction

| Aspect | Details |
|--------|---------|
| **Projects** | `HelloWinUI`, `WpfClientTests` |
| **Responsibility** | Display UI, capture user input, bind to ViewModel, handle UI events |
| **Key Components** | <ul><li>`MainWindow.xaml.cs` - UI handler</li><li>`SayHelloViewModel` - MVVM ViewModel (INotifyPropertyChanged)</li><li>`App.xaml.cs` - Bootstraps DI container</li></ul> |
| **Technologies** | WPF, XAML Data Binding, .NET 10 Windows |
| **Dependencies** | Model, Utils (DI configuration) |
| **Key Patterns** | MVVM, Data Binding, Dependency Injection |

**Example Code Flow:**
```csharp
// MainWindow.xaml (XAML Binding)
<TextBox Text="{Binding InputName}" />
<Button Click="RunWorkflow_Click" />

// MainWindow.xaml.cs (UI Handler)
private async void RunWorkflow_Click(object sender, RoutedEventArgs e) {
	var viewModel = this.DataContext as SayHelloViewModel;
	await viewModel.SendAsync();  // Delegates to ViewModel
}

// SayHelloViewModel.cs (Business Logic)
public async Task SendAsync() {
	// Executes Temporal workflow via ITemporalClient
	var result = await _client.ExecuteWorkflowAsync(
		(ISayHelloWorkflow wf) => wf.RunAsync(InputName),
		new(id: workflowId, _options.QueueName!)
	);
	GreetingResult = result;  // Updates UI via property binding
}
```

---

### Layer 2: **Workflow Layer** (Workflow)
**Purpose:** Define and orchestrate Temporal workflows

| Aspect | Details |
|--------|---------|
| **Projects** | `Workflow`, `WorkflowTests` |
| **Responsibility** | Define workflow logic, orchestrate activities, handle retries and timeouts |
| **Key Components** | <ul><li>`ISayHelloWorkflow` (Model.Ports) - Workflow interface</li><li>`SayHelloWorkflow` - Workflow implementation</li></ul> |
| **Technologies** | Temporal.io C# SDK (1.15.0) |
| **Dependencies** | Model (interfaces), Services (activities) |
| **Key Patterns** | Temporal Workflow Pattern, Activity Invocation |

**Example Code:**
```csharp
[Workflow]
public class SayHelloWorkflow : ISayHelloWorkflow {
	[WorkflowRun]
	public async Task<string> RunAsync(string name) {
		// Call activity within workflow
		var greeting = await Workflow.InvokeActivityAsync(
			(IGreetingActivity a) => a.GetGreetingAsync(name),
			new() { StartToCloseTimeout = TimeSpan.FromSeconds(30) }
		);
		return greeting;
	}
}
```

---

### Layer 3: **Services Layer** (Services)
**Purpose:** Business logic and external integrations

| Aspect | Details |
|--------|---------|
| **Projects** | `Services`, `ServicesTests` |
| **Responsibility** | Implement activities, contain reusable business logic, external API calls |
| **Key Components** | <ul><li>`GreetingActivity` - Temporal [Activity]</li><li>Domain-specific services</li></ul> |
| **Technologies** | Temporal.io Activities, .NET 10 |
| **Dependencies** | Model (interfaces and DTOs) |
| **Key Patterns** | Temporal Activity Pattern, Dependency Injection |

**Example Code:**
```csharp
public class GreetingActivity : IGreetingActivity {
	[Activity]
	public async Task<string> GetGreetingAsync(string name) {
		// Simulate business logic (e.g., database lookup, API call)
		return $"Hello, {name}! Welcome to Temporal.io";
	}
}
```

---

### Layer 4: **Worker Layer** (Worker)
**Purpose:** Host Temporal workflows and activities as a background service

| Aspect | Details |
|--------|---------|
| **Projects** | `Worker` (Console App) |
| **Responsibility** | Register workflows & activities, connect to Temporal Server, listen for work |
| **Key Components** | <ul><li>`Program.cs` - Worker startup and registration</li><li>Worker service registration</li></ul> |
| **Technologies** | Temporal.io C# SDK, Microsoft.Extensions.DependencyInjection |
| **Dependencies** | Utils, Workflow, Services |
| **Key Patterns** | Service Registration, Background Service |

**Example Code:**
```csharp
// Worker/Program.cs
var client = await TemporalClient.ConnectAsync(new(endpoint));

var worker = new TemporalWorker(client, new() {
	TaskQueue = "default"
});

worker.AddWorkflow<SayHelloWorkflow>();
worker.AddActivity<GreetingActivity>();

await worker.RunAsync();
```

---

### Layer 5: **Shared Layer** (Model, Utils, Client)
**Purpose:** Cross-cutting concerns, abstractions, and shared utilities

| Aspect | Details |
|--------|---------|
| **Projects** | `Model`, `Utils`, `Client` |
| **Responsibility** | Define interfaces, DTOs, configuration loading, DI setup, Temporal client initialization |
| **Key Components** | <ul><li>`ISayHelloWorkflow` - Workflow interface</li><li>`ExecutionWorkflowClient` - Configuration POCO</li><li>`ConfigurationBuilder` - Configuration loading</li><li>`TemporalClientFactory` - Client creation</li></ul> |
| **Technologies** | Microsoft.Extensions.Configuration, Microsoft.Extensions.DependencyInjection, Temporal.io |
| **Dependencies** | Base .NET libraries |
| **Key Patterns** | Port Interface Pattern, Factory Pattern, Configuration Pattern |

**Example Code:**
```csharp
// Model/Ports/ISayHelloWorkflow.cs (Port Interface)
[Workflow]
public interface ISayHelloWorkflow {
	[WorkflowRun]
	Task<string> RunAsync(string name);
}

// Utils/ConfigurationBuilder.cs
public static class ConfigurationBuilder {
	public static IConfiguration BuildConfiguration(string basePath) {
		return new ConfigurationBuilder()
			.SetBasePath(basePath)
			.AddJsonFile("appsettings.json")
			.Build();
	}
}
```

---

### Layer 6: **Testing Layer** (All *Tests projects)
**Purpose:** Verify functionality at each layer

| Aspect | Details |
|--------|---------|
| **Projects** | `WpfClientTests`, `WorkflowTests`, `ServicesTests` |
| **Responsibility** | Unit tests for ViewModels, Workflows, Activities, and Services |
| **Key Components** | <ul><li>Xunit test classes</li><li>Moq mocks for dependencies</li></ul> |
| **Technologies** | Xunit 3.2.2, Moq 4.20.72, Microsoft.NET.Test.Sdk 17.14.1 |
| **Dependencies** | Projects under test (HelloWinUI, Workflow, Services) |
| **Key Patterns** | Unit Testing, Mocking, AAA Pattern (Arrange-Act-Assert) |

**Example Code:**
```csharp
[Fact]
public async Task SendAsync_Should_InvokeTemporalWorkflow() {
	// Arrange
	var mockClient = new Mock<ITemporalClient>();
	var viewModel = new SayHelloViewModel(mockClient.Object, options);

	// Act
	await viewModel.SendAsync();

	// Assert
	Assert.NotEqual(string.Empty, viewModel.GreetingResult);
}
```

---

## 🔗 Project Dependencies

### Dependency Matrix

This table shows which projects **depend on** (require references to) other projects:

| Project | Depends On | Purpose |
|---------|-----------|---------|
| **HelloWinUI** | Model, Utils | DI setup, config loading, workflow interfaces |
| **WpfClientTests** | HelloWinUI, Model | Tests ViewModel layer |
| **Workflow** | Model | Uses ISayHelloWorkflow interface |
| **WorkflowTests** | Workflow, Model | Tests workflow implementations |
| **Services** | Model | Uses DTOs and interfaces |
| **ServicesTests** | Services | Tests activity implementations |
| **Worker** | Utils, Workflow, Services | Registers and hosts workflows & activities |
| **Client** | Workflow | Executes workflows via ITemporalClient |
| **Utils** | Services, Workflow | Centralizes DI and Temporal setup |
| **Model** | *(none)* | Base abstractions only |

### Dependency Graph

```
┌─────────────────────────────────────────────────────────────────┐
│                     PRESENTATION LAYER                          │
│  ┌─────────────────┐                                             │
│  │   HelloWinUI    │                                             │
│  │  (WinExe)       │─────────────────┐                           │
│  └─────────────────┘                 │                           │
│         ▲                            │                           │
│         │                            ▼                           │
│         │                    ┌──────────────┐                    │
│         │                    │    Model     │                    │
│         │                    │ (Interfaces) │                    │
│         │                    └──────────────┘                    │
│         │                            ▲                           │
│         └─────────────────────────────┘                          │
│              Uses ISayHelloWorkflow                              │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                      SHARED LAYER                               │
│  ┌─────────────┐       ┌──────────────┐                         │
│  │   Utils     │──────▶│   Workflow   │                         │
│  │ (DI/Config) │       │ (Definitions)│                         │
│  └─────────────┘       └──────────────┘                         │
│        ▲                      ▲                                  │
│        │                      │                                  │
│        │                ┌─────┴──────────┐                       │
│        └─────────────────│  Services      │                      │
│                          │  (Activities)  │                      │
│                          └────────────────┘                      │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                    WORKER LAYER                                 │
│  ┌──────────────┐                                               │
│  │    Worker    │──▶ Registers: Workflows + Activities           │
│  │   (Console)  │──▶ Hosts: Temporal Worker                      │
│  └──────────────┘                                               │
│        ▲                                                         │
│        └──────────────── Uses Utils, Workflow, Services          │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                    CLIENT LAYER                                 │
│  ┌──────────────┐                                               │
│  │    Client    │──▶ Executes: Workflows                        │
│  │   (Console)  │──▶ Via: ITemporalClient                        │
│  └──────────────┘                                               │
│        ▲                                                         │
│        └──────────────── Uses Workflow                           │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                   TESTING LAYERS                                │
│  ┌────────────────┐  ┌────────────────┐  ┌─────────────────┐   │
│  │ WpfClientTests │  │ WorkflowTests  │  │ ServicesTests   │   │
│  │ (Tests Views)  │  │ (Tests WF)     │  │ (Tests Services)│   │
│  └────────────────┘  └────────────────┘  └─────────────────┘   │
│        ▲                    ▲                      ▲             │
│        └────────────────────┴──────────────────────┘             │
│        Dependencies: HelloWinUI, Workflow, Services             │
└─────────────────────────────────────────────────────────────────┘
```

### Direct Project References

**HelloWinUI.csproj:**
```xml
<ProjectReference Include="..\Model\Model.csproj" />
<ProjectReference Include="..\Utils\Utils.csproj" />
```

**WpfClientTests.csproj:**
```xml
<ProjectReference Include="..\HelloWinUI\HelloWinUI.csproj" />
<ProjectReference Include="..\Model\Model.csproj" />
```

**Workflow.csproj:**
```xml
<ProjectReference Include="..\Model\Model.csproj" />
```

**WorkflowTests.csproj:**
```xml
<ProjectReference Include="..\Workflow\Workflow.csproj" />
<ProjectReference Include="..\Model\Model.csproj" />
```

**Services.csproj:**
```xml
<ProjectReference Include="..\Model\Model.csproj" />
```

**ServicesTests.csproj:**
```xml
<ProjectReference Include="..\Services\Services.csproj" />
```

**Worker.csproj:**
```xml
<ProjectReference Include="..\Utils\Utils.csproj" />
<ProjectReference Include="..\Workflow\Workflow.csproj" />
```

**Client.csproj:**
```xml
<ProjectReference Include="..\Workflow\Workflow.csproj" />
```

**Utils.csproj:**
```xml
<ProjectReference Include="..\Services\Services.csproj" />
<ProjectReference Include="..\Workflow\Workflow.csproj" />
```

---

## 📦 NuGet Dependencies

### By Project & Purpose

#### **Temporal.io Packages**
Used in: `Model`, `Workflow`, `Services`, `Worker`, `Client`, `Utils`
```
- Temporalio (v1.15.0) - Core Temporal.io C# SDK
- Temporalio.Extensions.Hosting (v1.15.0) - Hosting extensions for background workers
```

#### **Dependency Injection & Configuration**
Used in: `HelloWinUI`, `Worker`, `Client`, `Utils`
```
- Microsoft.Extensions.DependencyInjection (v10.0.9)
- Microsoft.Extensions.Configuration (v10.0.9)
- Microsoft.Extensions.Configuration.Abstractions (v10.0.9)
- Microsoft.Extensions.Configuration.Binder (v10.0.9)
- Microsoft.Extensions.Configuration.Json (v10.0.9)
```

#### **Testing Frameworks**
Used in: `WpfClientTests`, `WorkflowTests`, `ServicesTests`
```
- Xunit.v3 (v3.2.2) - Unit test framework
- Xunit.Runner.VisualStudio (v3.1.4) - VS integration
- Moq (v4.20.72) - Mocking library
- Microsoft.NET.Test.Sdk (v17.14.1) - Test SDK
- Coverlet.Collector (v6.0.4 - v10.0.1) - Code coverage
```

#### **WPF-Specific**
Used in: `HelloWinUI`, `WpfClientTests`
```
- Microsoft.NET.Sdk (built-in) - WPF framework
- UseWPF (MSBuild property) - Enables WPF support
```

### Complete NuGet Reference by Project

| Project | NuGet Packages | Count |
|---------|---|---|
| Model | Temporalio | 1 |
| Workflow | Temporalio | 1 |
| Services | *(none)* | 0 |
| HelloWinUI | Microsoft.Extensions.Configuration.Abstractions | 1 |
| Utils | Microsoft.Extensions.Configuration, Microsoft.Extensions.DependencyInjection, Temporalio.Extensions.Hosting | 3 |
| Worker | Microsoft.Extensions.Configuration.Binder, Microsoft.Extensions.Configuration.Json, Microsoft.Extensions.DependencyInjection, Temporalio | 4 |
| Client | Microsoft.Extensions.Configuration.Binder, Microsoft.Extensions.Configuration.Json, Temporalio | 3 |
| WpfClientTests | Xunit.v3, Xunit.Runner.VisualStudio, Moq, Microsoft.NET.Test.Sdk, Coverlet.Collector | 5 |
| WorkflowTests | Xunit.v3, Xunit.Runner.VisualStudio, Moq, Microsoft.NET.Test.Sdk, Coverlet.Collector | 5 |
| ServicesTests | Xunit.v3, Xunit.Runner.VisualStudio, Microsoft.NET.Test.Sdk, Coverlet.Collector | 4 |

---

## 🔄 Data Flow Diagrams

### Temporal Workflow Execution Flow (Happy Path)

```
┌─────────────────────────────────────────────────────────────────┐
│ WPF PRESENTATION LAYER                                          │
│                                                                 │
│  User Input (InputName)                                        │
│         │                                                       │
│         ▼                                                       │
│  MainWindow.RunWorkflow_Click()                                │
│         │                                                       │
│         ▼                                                       │
│  SayHelloViewModel.SendAsync()                                 │
│    ├─ IsProcessing = true                                      │
│    ├─ GreetingResult = "Executing..."                          │
└────────┼──────────────────────────────────────────────────────┘
		 │
		 │ ITemporalClient.ExecuteWorkflowAsync()
		 │
┌────────▼──────────────────────────────────────────────────────┐
│ TEMPORAL SERVER (localhost:7233)                               │
│                                                                 │
│  Creates Workflow Execution                                    │
│         │                                                       │
│         ▼                                                       │
│  SayHelloWorkflow.RunAsync(name)                               │
│         │                                                       │
│         ▼                                                       │
│  Invokes Activity: GreetingActivity.GetGreetingAsync(name)     │
└────────┼──────────────────────────────────────────────────────┘
		 │
┌────────▼──────────────────────────────────────────────────────┐
│ WORKER (Background Service)                                    │
│                                                                 │
│  Worker.ExecuteActivity()                                      │
│    ├─ Task: "default" queue                                    │
│    ├─ Activity: GreetingActivity                               │
│    ├─ Method: GetGreetingAsync(name)                           │
│    │                                                           │
│    ▼                                                           │
│  GreetingActivity.GetGreetingAsync()                           │
│    ├─ Performs business logic                                  │
│    ├─ Returns greeting string                                  │
│    │                                                           │
│    └─ Completes activity with result                           │
└────────┼──────────────────────────────────────────────────────┘
		 │
┌────────▼──────────────────────────────────────────────────────┐
│ WORKFLOW CONTINUATION                                          │
│                                                                 │
│  SayHelloWorkflow receives activity result                     │
│    └─ greeting = "Hello, {name}! Welcome to Temporal.io"       │
│                                                                 │
│  Returns from RunAsync()                                       │
└────────┼──────────────────────────────────────────────────────┘
		 │
		 │ Result: string (greeting)
		 │
┌────────▼──────────────────────────────────────────────────────┐
│ WPF PRESENTATION LAYER (Resumed)                               │
│                                                                 │
│  SayHelloViewModel.SendAsync() continues                       │
│    ├─ result = "Hello, Jose! Welcome to Temporal.io"           │
│    ├─ GreetingResult = result (updates UI)                     │
│    ├─ IsProcessing = false                                     │
│    │                                                           │
│    ▼                                                           │
│  XAML Binding                                                  │
│    └─ GreetingResult TextBlock updates on screen              │
└─────────────────────────────────────────────────────────────────┘
```

### WPF UI Interaction Flow

```
START: Application.OnStartup()
  │
  ├─ Load appsettings.json
  │  └─ Temporal Server endpoint: "localhost:7233"
  │
  ├─ Create ServiceCollection (DI Container)
  │  ├─ AddSingleton<ITemporalClient>(connectedClient)
  │  ├─ AddSingleton<ExecutionWorkflowClient>(options)
  │  ├─ AddTransient<SayHelloViewModel>()
  │  ├─ AddTransient<MainWindow>()
  │  └─ BuildServiceProvider()
  │
  └─ Resolve MainWindow from container
	 │
	 ▼
  MainWindow.ctor(SayHelloViewModel)
  ├─ DataContext = viewModel
  │  └─ XAML bindings now active:
  │     ├─ TextBox.Text ◄─ ViewModel.InputName
  │     └─ TextBlock.Text ◄─ ViewModel.GreetingResult
  │
  └─ Show()
	 │
	 ▼
  USER INTERACTION: Types name in TextBox
  │
  ├─ TextBox.Text updated
  │  └─ INotifyPropertyChanged
  │     └─ ViewModel.InputName = user input
  │
  └─ Clicks "Run Workflow" button
	 │
	 ▼
  MainWindow.RunWorkflow_Click()
  │
  ├─ Get DataContext as SayHelloViewModel
  │  └─ Invoke viewModel.SendAsync()
  │
  └─ SayHelloViewModel.SendAsync()
	 │
	 ├─ Validate: InputName not empty
	 │
	 ├─ Set IsProcessing = true
	 │  └─ TextBlock shows spinner (bound to IsProcessing)
	 │
	 ├─ Set GreetingResult = "Executing workflow..."
	 │  └─ UI updates immediately (bound property)
	 │
	 ├─ Create workflowId = $"{namespace}-{name.ToLower()}"
	 │
	 ├─ Call _client.ExecuteWorkflowAsync()
	 │  ├─ Sends execution request to Temporal Server
	 │  └─ Awaits result (async-await)
	 │
	 ├─ AWAIT: Workflow execution on Temporal Server
	 │  (UI remains responsive, dispatcher handles property changes)
	 │
	 ├─ Receive result from workflow
	 │  ├─ Update GreetingResult = result
	 │  │  └─ XAML binding updates TextBlock on UI thread
	 │  └─ Set IsProcessing = false
	 │     └─ Spinner removed, button re-enabled
	 │
	 └─ FINALLY: Catch exceptions, show error in GreetingResult
		└─ Update UI with error message
```

### Testing Strategy Pyramid

```
					▲
				   /|\
				  / | \
				 /  |  \
				/  E2E \            END-TO-END TESTS
			   /    |    \          (Not included in this project)
			  /     |     \         - Full workflow execution
			 /      |      \        - Real Temporal Server
			/       |       \
		   ┌────────────────┐
		   │   20%         │       INTEGRATION TESTS
		   │ WORKFLOW TESTS│       - Test Workflows with mocked Activities
		   │ (WorkflowTests)│      - Verify workflow orchestration logic
		   └────────────────┘
				  /  \
				 /    \
				/      \
			   /        \
			  ┌──────────────────┐
			  │      40%         │  UNIT TESTS (MAIN FOCUS)
			  │  SERVICE TESTS   │  - Activity tests (ServicesTests)
			  │  (ServicesTests) │  - ViewModel tests (WpfClientTests)
			  │  ViewModel Tests │  - Mocked dependencies (Moq)
			  │ (WpfClientTests) │  - Fast execution
			  └──────────────────┘
					   ▲
					   │
				   Many Tests
				  Fast & Isolated
```

**Test Distribution:**

| Layer | Test Project | Count | Type | Technologies |
|-------|---|---|---|---|
| **Services** | ServicesTests | ~5-10 tests | Unit | Xunit, Moq |
| **Workflows** | WorkflowTests | ~5-10 tests | Integration | Xunit, Moq |
| **ViewModels** | WpfClientTests | ~5-10 tests | Unit | Xunit, Moq |
| **Total** | All Test Projects | ~15-30 tests | Mixed | Xunit, Moq, Coverlet |

---

## 🏗️ Architecture Patterns & Principles

### 1. **MVVM Pattern (Presentation Layer)**
- **View**: `MainWindow.xaml` - Pure XAML, no code-behind logic
- **ViewModel**: `SayHelloViewModel` - Implements `INotifyPropertyChanged`, manages state and commands
- **Model**: `ExecutionWorkflowClient` - Configuration and workflow options

**Benefits:**
- Testable UI logic (ViewModel can be tested without WPF)
- Data binding updates automatically
- Clear separation of UI and business logic

### 2. **Port Interface Pattern (Shared Layer)**
- **Port**: `ISayHelloWorkflow` - Interface defined in Model
- **Adapter**: `SayHelloWorkflow` - Implementation in Workflow project

**Benefits:**
- Loose coupling between presentation and workflow layers
- Easy to mock for testing
- Enables multiple implementations if needed

### 3. **Dependency Injection (All Layers)**
- Microsoft.Extensions.DependencyInjection
- Configured in `App.xaml.cs`
- Singleton: `ITemporalClient`, `ExecutionWorkflowClient`
- Transient: `SayHelloViewModel`, `MainWindow`

**Benefits:**
- Testable components (inject mocks in tests)
- Centralized configuration
- Loose coupling between components

### 4. **Factory Pattern (Utils Layer)**
- `TemporalClientFactory.CreateClientAsync()` - Creates and configures `ITemporalClient`

**Benefits:**
- Encapsulates complex client initialization
- Reusable across multiple projects (HelloWinUI, Worker, Client)

### 5. **Activity Pattern (Temporal.io)**
- Activities: `GreetingActivity` - Perform business logic asynchronously
- Workflows: `SayHelloWorkflow` - Orchestrate activities

**Benefits:**
- Activities are retryable and durable
- Workflow logic is deterministic
- Automatic error handling and recovery

### 6. **Configuration Pattern (Utils Layer)**
- `ConfigurationBuilder.cs` - Centralized configuration loading
- `appsettings.json` in each executable project

**Benefits:**
- Single source of truth for settings
- Easy to change environment-specific configs (dev, prod)
- Supports multiple configuration sources (JSON, environment variables, etc.)

---

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- Temporal Server running on `localhost:7233` (or update appsettings.json)
- Visual Studio 2026 or VS Code

### Running the Application

#### 1. Start Temporal Server
```powershell
# Using Docker (if installed)
docker run -p 7233:7233 --name temporal temporalio/temporal:latest

# Or download and run temporalite (single-machine version)
# https://temporal.io/blog/temporalite
```

#### 2. Start the Temporal Worker
```powershell
cd Worker
dotnet run
# Output: Worker is now listening for tasks on "default" queue
```

#### 3. Run the WPF Application
```powershell
cd HelloWinUI
dotnet run
# MainWindow opens, ready for user input
```

#### 4. Execute a Workflow
1. Type your name in the TextBox
2. Click "Run Workflow" button
3. Watch the greeting result appear

#### 5. (Optional) Run Tests
```powershell
# Run all tests
dotnet test

# Run specific test project
dotnet test WpfClientTests
dotnet test WorkflowTests
dotnet test ServicesTests
```

### Configuration
Update `appsettings.json` files to change Temporal Server endpoint:

**HelloWinUI/appsettings.json:**
```json
{
  "Configurations": {
	"Endpoint": "localhost:7233",
	"QueueName": "default",
	"WorkflowNamespaceId": "default"
  }
}
```

**Worker/appsettings.json:**
```json
{
  "Temporal": {
	"Endpoint": "localhost:7233",
	"QueueName": "default"
  }
}
```

---

## 📚 Learning Goals & Resources

### Goals Demonstrated in This Project
✅ **WPF Fundamentals**
- XAML markup and MVVM pattern
- Data binding and `INotifyPropertyChanged`
- Async/await with UI thread safety
- Dependency Injection in WPF

✅ **Temporal.io Workflow Orchestration**
- Workflow definition and execution
- Activity invocation and error handling
- Distributed workflow concepts
- Temporal Server interaction

✅ **Clean Architecture**
- Layered architecture (6 layers)
- Dependency injection patterns
- Port interface pattern
- Separation of concerns

✅ **Testing**
- Unit testing with Xunit
- Mocking with Moq
- Testable architecture design
- Testing each layer independently

### Recommended Resources
- **Temporal.io C# SDK**: https://docs.temporal.io/develop/csharp
- **WPF Documentation**: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/
- **MVVM Pattern**: https://learn.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern
- **.NET 10 Release Notes**: https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10
- **Xunit Testing**: https://xunit.net/

---

## 📝 License

This is a personal learning project for practicing WPF and Temporal.io.

---

## 📞 Notes

- **Workflow Determinism**: Temporal workflows must be deterministic (no random operations, dates, etc. outside activities)
- **Activity Retries**: Activities are retried automatically on failure; ensure they are idempotent
- **UI Thread Safety**: `SynchronizationContext` is captured in ViewModel to safely update UI from workflow results
- **Configuration Loading**: All projects load appsettings.json from the output directory

---

**Last Updated**: 2024  
**Target Runtime**: .NET 10  
**IDE**: Visual Studio 2026
