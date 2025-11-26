# XmlDotnetCoding Project

This project focuses on C# development for reading and processing XML files into C# classes.

## Project Overview

This is a .NET/C# development project with a focus on:
- XML parsing and deserialization
- C# class design for XML data structures
- XML serialization techniques
- Best practices for handling XML in .NET applications

## Specialized Agents

This project includes three specialized agents in `.claude/agents/`:

### csharp-xml-expert
Use this agent for XML-specific tasks:
- Designing C# classes to map to XML structures
- Choosing the right XML parsing approach (XmlSerializer, LINQ to XML, XmlReader)
- Handling XML namespaces and attributes
- Implementing XML serialization and deserialization
- Working with XSD schemas

### csharp-reviewer
Use this agent for code review tasks:
- Reviewing C# code quality and patterns
- Identifying bugs and performance issues
- Ensuring adherence to C# best practices
- Verifying proper error handling
- Checking use of modern C# features

### dotnet-tester
Use this agent for testing tasks:
- Writing unit tests (xUnit, NUnit, MSTest)
- Creating test fixtures for XML scenarios
- Implementing test data builders
- Ensuring good test coverage
- Debugging failing tests

## Project-Specific Guidelines

### C# Code Standards
- Use modern C# features (nullable reference types, file-scoped namespaces, etc.)
- Follow C# naming conventions (PascalCase for classes/methods, camelCase for variables)
- Add XML documentation comments to public APIs
- Use proper async/await patterns where applicable

### XML Processing Approach
- Analyze XML structure before designing classes
- Choose appropriate parsing method based on requirements:
  - **XmlSerializer**: For straightforward object mapping
  - **LINQ to XML**: For flexible querying and manipulation
  - **XmlReader**: For large files requiring memory efficiency
- Always implement robust error handling for XML parsing
- Validate XML against schemas when available

### File Organization
- Keep XML sample files in a dedicated directory (e.g., `samples/`, `testdata/`)
- Organize classes logically (models, parsers, utilities)
- Use appropriate project structure for .NET applications

### Testing Requirements
- Write unit tests for XML parsing logic
- Test both happy path and error scenarios
- Include tests for different XML structure variations
- Use embedded resources for sample XML files in tests

## Development Workflow

1. **Analyze XML Structure**: Examine sample XML files to understand the data model
2. **Design Classes**: Create C# classes with appropriate XML attributes
3. **Implement Parsers**: Write code to deserialize XML to objects
4. **Add Error Handling**: Implement robust exception handling
5. **Write Tests**: Create comprehensive unit tests
6. **Review Code**: Use the csharp-reviewer agent for code review
7. **Refactor**: Improve code based on review feedback

## Common Tasks

### Creating Classes from XML
```csharp
// Use XML serialization attributes
[XmlRoot("RootElement")]
public class MyClass
{
    [XmlElement("ChildElement")]
    public string Property { get; set; }

    [XmlAttribute("attributeName")]
    public int AttributeValue { get; set; }
}
```

### Deserializing XML
```csharp
// Using XmlSerializer
var serializer = new XmlSerializer(typeof(MyClass));
using var reader = new StreamReader("file.xml");
var result = (MyClass)serializer.Deserialize(reader);
```

### Using LINQ to XML
```csharp
// Load and query XML
var doc = XDocument.Load("file.xml");
var elements = doc.Descendants("ElementName")
    .Where(e => e.Attribute("attr")?.Value == "value");
```

## Dependencies and Tools

- **.NET SDK**: Ensure appropriate .NET version is installed
- **NuGet Packages**: Install required packages (testing frameworks, etc.)
- **XML Tools**: Consider using xsd.exe for generating classes from XSD schemas

## Notes

- Prefer editing existing files over creating new ones
- Follow the shared principles in `/terminai/CLAUDE.md`
- Use the TodoWrite tool for tracking multi-step tasks
- Ask clarifying questions before making assumptions about XML structure or requirements
