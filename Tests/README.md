# Testes do poupeai-report-service

Este documento descreve a estrutura de testes e como executá-los.

## Casos de Teste Implementados

### Testes Unitários (Unit Tests)

| ID | Módulo | Descrição | Status |
|---|---|---|---|
| RS-UT-001 | Utils (Hash) | Verificar consistência da geração de hash | Implementado e Passando (3 testes) |
| RS-UT-002 | Utils (Tools) | Verificar deserialização de JSON válido retornado pela IA | Implementado e Passando |
| RS-UT-003 | Utils (Tools) | Verificar tratamento de erro na deserialização de JSON inválido | Implementado e Passando |
| RS-UT-004 | Service (SavingsEstimate) | Verificar separação correta de transações por período | Simplificado (testado via dados) |
| RS-UT-005 | Service (ExpenseService) | Verificar construção correta do prompt para relatório de despesas | Implementado e Passando (2 testes) |
| RS-UT-006 | Service (BaseReportService) | Verificar lógica de cache hit | Simplificado (teste de modelo) |
| RS-UT-007 | Service (BaseReportService) | Verificar lógica de cache miss | Simplificado (teste de deserialização) |
| RS-UT-008 | Service (GeminiAIService) | Verificar lógica de retry em caso de erro 429/503 | Implementado e Passando (4 testes) |
| RS-UT-009 | Service (CategoryService) | Verificar BuildPrompt com múltiplas categorias | Pendente |
| RS-UT-010 | Service (CategoryService) | Verificar MapResponseToModel com dados válidos | Pendente |
| RS-UT-011 | Service (CategoryService) | Verificar MapResponseToModel com dados inválidos | Pendente |
| RS-UT-012 | Service (CategoryService) | Verificar cache hit com hash específico de categoria | Pendente |
| RS-UT-013 | Service (CategoryService) | Verificar tratamento de erro quando categoria não existe | Pendente |
| RS-UT-014 | Service (CategoryService) | Verificar GenerateReportAsync com lista vazia de transações | Pendente |
| RS-UT-015 | Service (CategoryService) | Verificar timeout na chamada da IA | Pendente |
| RS-UT-016 | Service (CategoryService) | Verificar erro ao salvar no MongoDB | Pendente |
| RS-UT-017 | Service (IncomeService) | Verificar BuildPrompt para relatório de receitas | Pendente |
| RS-UT-018 | Service (IncomeService) | Verificar MapResponseToModel com receitas válidas | Pendente |
| RS-UT-019 | Service (IncomeService) | Verificar separação de receitas principais | Pendente |
| RS-UT-020 | Service (IncomeService) | Verificar cálculo de percentuais de receita | Pendente |
| RS-UT-021 | Service (IncomeService) | Verificar cache hit para relatório de receitas | Pendente |
| RS-UT-022 | Service (IncomeService) | Verificar tratamento de período sem receitas | Pendente |
| RS-UT-023 | Service (IncomeService) | Verificar GenerateReportAsync com erro da IA | Pendente |
| RS-UT-024 | Service (IncomeService) | Verificar validação de datas de período | Pendente |
| RS-UT-025 | Service (InsightService) | Verificar BuildPrompt com InsightReportRequest | Pendente |
| RS-UT-026 | Service (InsightService) | Verificar MapResponseToModel para insights | Pendente |
| RS-UT-027 | Service (InsightService) | Verificar geração de insights com foco específico | Pendente |
| RS-UT-028 | Service (InsightService) | Verificar cache hit baseado em hash de request | Pendente |
| RS-UT-029 | Service (InsightService) | Verificar tratamento de insights vazios | Pendente |
| RS-UT-030 | Service (InsightService) | Verificar GenerateReportAsync com múltiplos focos | Pendente |
| RS-UT-031 | Service (InsightService) | Verificar erro de deserialização de insights | Pendente |
| RS-UT-032 | Service (InsightService) | Verificar timeout na geração de insights | Pendente |
| RS-UT-033 | Service (OverviewService) | Verificar BuildPrompt para visão geral | Pendente |
| RS-UT-034 | Service (OverviewService) | Verificar MapResponseToModel com overview completo | Pendente |
| RS-UT-035 | Service (OverviewService) | Verificar cálculo de saldo (receitas - despesas) | Pendente |
| RS-UT-036 | Service (OverviewService) | Verificar identificação de tendências | Pendente |
| RS-UT-037 | Service (OverviewService) | Verificar GenerateReportAsync com dados zerados | Pendente |
| RS-UT-038 | Service (OverviewService) | Verificar cache e atualização de overview | Pendente |
| RS-UT-039 | Service (SavingsEstimateService) | Verificar BuildPrompt com comparison semanal | Pendente |
| RS-UT-040 | Service (SavingsEstimateService) | Verificar BuildPrompt com comparison anual | Pendente |
| RS-UT-041 | Service (SavingsEstimateService) | Verificar cálculo de economia mensal | Pendente |
| RS-UT-042 | Service (SavingsEstimateService) | Verificar cálculo de economia semanal | Pendente |
| RS-UT-043 | Service (SavingsEstimateService) | Verificar cálculo de economia anual | Pendente |
| RS-UT-044 | Service (SavingsEstimateService) | Verificar comparação de períodos iguais | Pendente |
| RS-UT-045 | Service (SavingsEstimateService) | Verificar tratamento de períodos incompletos | Pendente |
| RS-UT-046 | Service (SavingsEstimateService) | Verificar sugestões de economia geradas | Pendente |
| RS-UT-047 | Service (SavingsEstimateService) | Verificar erro com comparisonType inválido | Pendente |
| RS-UT-048 | Service (SavingsEstimateService) | Verificar GenerateReportAsync sem transações | Pendente |
| RS-UT-049 | Service (FinanceService) | Verificar FetchTransactionsAsync com sucesso | Pendente |
| RS-UT-050 | Service (FinanceService) | Verificar tratamento de erro 401 (não autorizado) | Pendente |
| RS-UT-051 | Service (FinanceService) | Verificar tratamento de erro 500 (erro do servidor) | Pendente |
| RS-UT-052 | Service (FinanceService) | Verificar retry em caso de timeout | Pendente |
| RS-UT-053 | Service (FinanceService) | Verificar parsing de resposta JSON | Pendente |
| RS-UT-054 | Service (FinanceService) | Verificar Headers JWT corretos na requisição | Pendente |
| RS-UT-055 | Service (DeepseekAIService) | Verificar GenerateReportAsync com sucesso | Pendente |
| RS-UT-056 | Service (DeepseekAIService) | Verificar retry com erro 429 | Pendente |
| RS-UT-057 | Service (DeepseekAIService) | Verificar tratamento de erro 500 | Pendente |
| RS-UT-058 | Service (DeepseekAIService) | Verificar parsing de DeepseekResponse | Pendente |
| RS-UT-059 | Service (DeepseekAIService) | Verificar timeout da API | Pendente |
| RS-UT-060 | Service (DeepseekAIService) | Verificar Headers corretos (API Key) | Pendente |
| RS-UT-061 | Service (AIServiceAggregator) | Verificar seleção de Gemini quando especificado | Pendente |
| RS-UT-062 | Service (AIServiceAggregator) | Verificar seleção de Deepseek quando especificado | Pendente |
| RS-UT-063 | Service (AIServiceAggregator) | Verificar fallback para Gemini se Deepseek falhar | Pendente |
| RS-UT-064 | Service (AIServiceAggregator) | Verificar fallback para Deepseek se Gemini falhar | Pendente |
| RS-UT-065 | Service (AIServiceAggregator) | Verificar erro quando ambos os serviços falham | Pendente |
| RS-UT-066 | Service (AIServiceAggregator) | Verificar GenerateReportAsync com AIModel inválido | Pendente |
| RS-UT-067 | Service (AIServiceAggregator) | Verificar logging de falhas e fallbacks | Pendente |
| RS-UT-068 | Service (AIServiceAggregator) | Verificar seleção aleatória quando AIModel não especificado | Pendente |
| RS-UT-069 | Service (BaseReportService) | Verificar erro ao inserir no MongoDB (duplicata) | Pendente |
| RS-UT-070 | Service (BaseReportService) | Verificar erro de comunicação com MongoDB | Pendente |
| RS-UT-071 | Service (BaseReportService) | Verificar timeout na busca do cache | Pendente |
| RS-UT-072 | Service (BaseReportService) | Verificar validação de TransactionsData nulo | Pendente |
| RS-UT-073 | Models (CategoryReportModel) | Verificar propriedades de CategoryReportModel | Pendente |
| RS-UT-074 | Models (ExpenseReportModel) | Verificar propriedades de ExpenseReportModel | Pendente |
| RS-UT-075 | Models (IncomeReportModel) | Verificar propriedades de IncomeReportModel | Pendente |
| RS-UT-076 | Models (InsightReportModel) | Verificar propriedades de InsightReportModel | Pendente |
| RS-UT-077 | Models (OverviewReportModel) | Verificar propriedades de OverviewReportModel | Pendente |
| RS-UT-078 | Models (TransactionModel) | Verificar propriedades de TransactionModel | Pendente |
| RS-UT-079 | Models (CategoryModel) | Verificar CategoryModel com categorias válidas | Pendente |
| RS-UT-080 | Models (BaseReportModel) | Verificar BaseReportModel com hash correto | Pendente |
| RS-UT-081 | Models | Verificar serialização/deserialização de Models | Pendente |
| RS-UT-082 | Models | Verificar validação de datas em Models | Pendente |
| RS-UT-083 | Models | Verificar valores default em Models | Pendente |
| RS-UT-084 | Models | Verificar conversão de Models para MongoDB | Pendente |
| RS-UT-085 | DTOs (CategoryReportRequest) | Verificar CategoryReportRequest com múltiplas categorias | Pendente |
| RS-UT-086 | DTOs (InsightReportRequest) | Verificar InsightReportRequest com focus válido | Pendente |
| RS-UT-087 | DTOs (Transaction) | Verificar Transaction com todos os campos | Pendente |
| RS-UT-088 | DTOs (TransactionsData) | Verificar TransactionsData com período completo | Pendente |
| RS-UT-089 | DTOs (Requests) | Verificar validação de datas em Requests | Pendente |
| RS-UT-090 | DTOs (Requests) | Verificar serialização JSON de Requests | Pendente |
| RS-UT-091 | DTOs (AIResponse) | Verificar AIResponse<T> genérico | Pendente |
| RS-UT-092 | DTOs (DeepseekResponse) | Verificar DeepseekResponse parsing | Pendente |
| RS-UT-093 | DTOs (GeminiResponse) | Verificar GeminiResponse parsing | Pendente |
| RS-UT-094 | DTOs (SavingsEstimateResponse) | Verificar SavingsEstimateResponse cálculos | Pendente |
| RS-UT-095 | DTOs (CategoryReportResponse) | Verificar CategoryReportResponse estrutura | Pendente |
| RS-UT-096 | DTOs (ExpenseReportResponse) | Verificar ExpenseReportResponse estrutura | Pendente |
| RS-UT-097 | DTOs (IncomeReportResponse) | Verificar IncomeReportResponse estrutura | Pendente |
| RS-UT-098 | DTOs (InsightReportResponse) | Verificar InsightReportResponse estrutura | Pendente |
| RS-UT-099 | DTOs (OverviewReportResponse) | Verificar OverviewReportResponse estrutura | Pendente |
| RS-UT-100 | DTOs (TransactionReportResponse) | Verificar TransactionReportResponse estrutura | Pendente |

### Testes de Integração (Integration Tests)

| ID | Módulo | Descrição | Status |
|---|---|---|---|
| RS-IC-001 | API (ReportsRoutes) | Testar endpoint /overview (happy path) com dependências mockadas | Pendente |
| RS-IC-002 | API (ReportsRoutes) | Testar endpoint /overview com cache hit | Pendente |
| RS-IC-003 | API (ReportsRoutes) | Testar endpoint /expense sem token JWT | Pendente |
| RS-IC-004 | API (ReportsRoutes) | Testar endpoint /income quando FinancesService falha | Pendente |
| RS-IC-005 | API (ReportsRoutes) | Testar endpoint /category quando AI retorna JSON inválido | Pendente |
| RS-IC-006 | API (SavingsRoutes) | Testar endpoint /savings/estimate (happy path) | Pendente |
| RS-IC-007 | API (SavingsRoutes) | Testar endpoint /savings/estimate com comparisonType inválido | Pendente |

## Estrutura do Projeto de Testes

```
Tests/
├── Helpers/
│   └── TestBase.cs                          # Classe base com mocks comuns
├── Utils/
│   ├── HashTests.cs                         # RS-UT-001
│   └── ToolsTests.cs                        # RS-UT-002, RS-UT-003
├── Services/
│   ├── SavingsEstimateServiceTests.cs       # RS-UT-004
│   ├── ExpenseServiceTests.cs               # RS-UT-005
│   ├── BaseReportServiceTests.cs            # RS-UT-006, RS-UT-007
│   └── AI/
│       └── GeminiAIServiceTests.cs          # RS-UT-008
└── Integration/                             # Testes de integração (pendente)
```

## Tecnologias e Frameworks Utilizados

- **xUnit**: Framework de testes
- **Moq**: Biblioteca para criação de mocks
- **Moq.Contrib.HttpClient**: Extensão do Moq para mockar HttpClient
- **FluentAssertions**: Assertions fluentes e legíveis
- **Coverlet**: Ferramenta de cobertura de código

## Resultado dos Testes

**Última execução:**
- **Total**: 33 testes
- **Passando**: 33 testes (100%)
- **Falhando**: 0 testes
- **Ignorados**: 0 testes
- **Duração**: ~5.8s

### Distribuição por Categoria

- **Utils (Hash)**: 3 testes 
- **Utils (Tools)**: 11 testes 
- **Services (SavingsEstimate)**: 1 teste 
- **Services (Expense)**: 2 testes 
- **Services (BaseReport)**: 2 testes 
- **Services (GeminiAI)**: 4 testes 
- **Testes de StringToModel/ModelToString**: 10 testes 

## Como Executar os Testes

### Executar todos os testes

```bash
dotnet test
```

### Executar testes com cobertura

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Executar testes de uma categoria específica

```bash
# Executar apenas testes unitários
dotnet test --filter "Category=Unit"

# Executar apenas testes de integração
dotnet test --filter "Category=Integration"
```

### Executar um caso de teste específico

```bash
# Executar apenas o caso de teste RS-UT-001
dotnet test --filter "TestCase=RS-UT-001"
```

### Gerar relatório de cobertura em HTML

```bash
# Instalar a ferramenta ReportGenerator (primeira vez apenas)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory ./coverage-output

# Gerar relatório HTML
reportgenerator -reports:"./coverage-output/**/coverage.cobertura.xml" -targetdir:"./coverage-report" -reporttypes:Html
```

O relatório HTML estará disponível em `./coverage-report/index.html`

## Padrões de Nomenclatura

### Nomenclatura de Testes

```csharp
[Fact]
[Trait("Category", "Unit")]
[Trait("TestCase", "RS-UT-001")]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    // ...

    // Act
    // ...

    // Assert
    // ...
}
```

- **Trait("Category")**: Define se é teste unitário (Unit) ou de integração (Integration)
- **Trait("TestCase")**: Referência ao ID do caso de teste
- **Nome do método**: Segue o padrão `MethodName_Scenario_ExpectedBehavior`

### Organização do Teste (AAA Pattern)

Todos os testes seguem o padrão AAA:
- **Arrange**: Configuração do cenário e mocks
- **Act**: Execução do método sendo testado
- **Assert**: Verificação dos resultados

## Metas de Cobertura

- **Meta Geral**: 80% de cobertura de código
- **Componentes críticos**: 90%+ (Services, Utils)
- **DTOs e Models**: 70%+ (focado em lógica de negócio)

## Recursos Adicionais

- [xUnit Documentation](https://xunit.net/)
- [Moq Quick Start](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)
