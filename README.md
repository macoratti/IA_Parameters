# Console LLM Tester - GitHub Models + Ollama (C#)

Um aplicativo console simples em **C# (.NET 8+)** que permite testar modelos de linguagem (LLMs) de duas formas:

- **GitHub Models** (gpt-4o-mini via Azure OpenAI-compatible endpoint)
- **Ollama** (modelos locais, exemplo com llama3.1)

Tudo usando a nova abstração unificada do **Microsoft.Extensions.AI** (`IChatClient`), permitindo trocar de provedor com apenas uma linha.

## Funcionalidades

- Seleção do provedor no início (GitHub Models ou Ollama)
- Menu interativo para ajustar parâmetros do modelo em tempo real:
  - Max Tokens
  - Temperature
  - Top-P
  - Frequency Penalty
  - Presence Penalty
  - Stop Sequence
- Envio de prompts com exibição de uso de tokens
- Interface colorida e amigável no console

## Pré-requisitos

- .NET 8 SDK
- Ollama instalado e rodando (se for usar a opção local) → https://ollama.com
- Um Personal Access Token (PAT) do GitHub com o scope `read:packages` (para usar GitHub Models)

## Pacotes NuGet utilizados

```xml
<PackageReference Include="Microsoft.Extensions.AI" Version="8.*" />
<PackageReference Include="Microsoft.Extensions.AI.Abstractions" Version="8.*" />
<PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="8.*" />
<PackageReference Include="OllamaSharp" Version="2.*" />
<PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="8.*" />

Configuração do GitHub Models

Crie um PAT no GitHub com a permissão read:packages
No projeto, adicione o secret local:

dotnet user-secrets init
dotnet user-secrets set "GH_PAT" "github_pat_xxxxxxxxxxxxxxxxxxxxx"

Como executar  : dotnet run

Ao iniciar, escolha:

1 - GitHub Models (gpt-4o-mini)
2 - Ollama (llama3.1)

Por que usar Microsoft.Extensions.AI?

Código único para múltiplos provedores (OpenAI, Azure, Ollama, Anthropic, etc.)

Abstração IChatClient padronizada

Fácil migração futura para outros backends


