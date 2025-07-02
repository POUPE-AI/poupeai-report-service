# PoupeAI Report Service

> Microserviço de geração de relatórios financeiros inteligentes usando IA

Este é um dos microserviços do projeto integrador **PoupeAI**, desenvolvido para a disciplina de Sistemas Distribuídos do curso de Análise e Desenvolvimento de Sistemas (ADS) do IFRN. O serviço é responsável por gerar relatórios financeiros personalizados utilizando modelos de IA para análise de transações.

## 📋 Sobre o Projeto

O **PoupeAI Report Service** analisa dados de transações financeiras e gera relatórios inteligentes com insights personalizados, sugestões de melhoria e análises detalhadas sobre o comportamento financeiro do usuário.

### Funcionalidades Principais

- 📊 **Relatórios de Despesas**: Análise detalhada de gastos por categoria
- 💰 **Relatórios de Receitas**: Acompanhamento de entradas financeiras
- 🏷️ **Relatórios por Categoria**: Análise específica por categoria de transação
- 🔍 **Insights Personalizados**: Relatórios customizados baseados em perguntas do usuário
- 🎯 **Sugestões Inteligentes**: Recomendações para melhorar a saúde financeira
- 💾 **Cache Inteligente**: Sistema de cache baseado em hash para otimizar performance

## 🛠️ Tecnologias Utilizadas

- **Framework**: ASP.NET Core 8.0
- **Linguagem**: C#
- **Banco de Dados**: MongoDB
- **IA/ML**: Google Gemini AI, DeepSeek AI
- **Logging**: Serilog com integração Grafana Loki
- **Containerização**: Docker
- **Documentação**: Swagger/OpenAPI

## 🚀 Como Instalar e Executar

### Pré-requisitos

- .NET 8.0 SDK
- MongoDB
- Chaves de API do Google Gemini AI e/ou DeepSeek AI

### Instalação

1. **Clone o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd poupeai-report-service
   ```

2. **Configure as variáveis de ambiente**
   
   Crie um arquivo `appsettings.Development.json` ou configure as seguintes variáveis:
   ```json
   {
     "Database": {
       "ConnectionString": "mongodb://localhost:27017",
       "DatabaseName": "poupeai-reports"
     },
     "GeminiAI": {
       "ApiKey": "sua-chave-gemini-api"
     },
     "DeepseekAI": {
       "ApiKey": "sua-chave-deepseek-api",
       "BaseUrl": "https://api.deepseek.com"
     }
   }
   ```

3. **Execute o projeto**
   ```bash
   dotnet restore
   dotnet run
   ```

### Usando Docker

```bash
docker build -t poupeai-report-service .
docker run -p 5095:8080 poupeai-report-service
```

### Modelos de IA Disponíveis

- `gemini` - Google Gemini 2.0 Flash (padrão)
- `deepseek` - DeepSeek AI

## 🏗️ Arquitetura

O serviço segue uma arquitetura modular com:

- **Controllers/Routes**: Definição dos endpoints da API
- **Services**: Lógica de negócio para cada tipo de relatório
- **AI Services**: Integração com diferentes provedores de IA
- **Models**: Modelos de dados para MongoDB
- **DTOs**: Objetos de transferência de dados
- **Utils**: Utilitários e helpers

## 📊 Recursos Avançados

- **Sistema de Cache**: Evita regerar relatórios idênticos
- **Retry Logic**: Tentativas automáticas em caso de falha da IA
- **Tratamento de Erros**: Respostas padronizadas para diferentes cenários
- **Logging Estruturado**: Logs detalhados para monitoramento
- **Health Checks**: Monitoramento da saúde do serviço

## 🤝 Projeto Integrador - IFRN

Este microserviço faz parte do projeto integrador da disciplina de Sistemas Distribuídos do curso de **Análise e Desenvolvimento de Sistemas** do **Instituto Federal do Rio Grande do Norte (IFRN)**.

## 📝 Documentação da API

Acesse a documentação interativa da API em:
```
http://localhost:5095/swagger
```

## 🐛 Contribuição

Este é um projeto acadêmico, mas sugestões e melhorias são bem-vindas através de issues e pull requests.

---

**Desenvolvido para o IFRN - Curso de ADS**
