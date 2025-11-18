using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OllamaSharp;
using System.ClientModel;

class Program
{
    // ========================
    // PARÂMETROS DO LLM
    // ========================
    static int maxTokens = 100;
    static double temperature = 0.7;
    static double topP = 1.0;
    static double freqPenalty = 0.0;
    static double presencePenalty = 0.0;
    static string? stopSequence = null;

    static async Task Main()
    {
        Console.Title = "LLMs - GitHub Models + Ollama (llama3.1)";

        // ========================
        // CONFIGURAÇÃO DO CLIENTE GITHUB
        // ========================
        IConfigurationRoot config = new ConfigurationBuilder()
                                        .AddUserSecrets<Program>()
                                        .Build();

        // ========================
        // SELECIONAR PROVEDOR
        // ========================
        Console.WriteLine("\n === Selecione um provedor de IA ===\n");
        Console.WriteLine("1 - GitHub Models (gpt-4o-mini)");
        Console.WriteLine("2 - Ollama (llama3.1)");
        Console.Write("\nOpção: ");
        string escolha = Console.ReadLine()?.Trim() ?? "1";

        IChatClient client;

        if (escolha == "2")
        {
            // ========================
            // OLLAMA LOCAL
            // ========================
            var ollama = new OllamaApiClient(new Uri("http://localhost:11434"))
            {
                SelectedModel = "llama3.1" // ajuste se necessário
            };

            client = ollama; // já implementa IChatClient
        }
        else
        {
            // ========================
            // GITHUB MODELS
            // ========================
            var credential = new ApiKeyCredential(
                config["GH_PAT"]
                ?? throw new InvalidOperationException("Configure GH_PAT no User Secrets")
            );

            var options = new OpenAIClientOptions()
            {
                Endpoint = new Uri("https://models.github.ai/inference")
            };

            client =
               new OpenAIClient(credential, options)
                   .GetChatClient("openai/gpt-4o-mini")
                   .AsIChatClient();
        }

        // ========================
        // LOOP DE MENU
        // ========================
        while (true)
        {
            Console.Clear();
            MostrarParametros();
            MostrarMenu();

            Console.Write("\nEscolha uma opção: ");
            string opcao = Console.ReadLine()?.Trim() ?? "";

            switch (opcao)
            {
                case "1": AlterarMaxTokens(); break;
                case "2": AlterarTemperature(); break;
                case "3": AlterarTopP(); break;
                case "5": AlterarFrequencyPenalty(); break;
                case "6": AlterarPresencePenalty(); break;
                case "7": AlterarStopSequence(); break;
                case "8": await FazerPergunta(client); break;
                case "0": return;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Opção inválida.");
                    Console.ResetColor();
                    Thread.Sleep(1200);
                    break;
            }
        }
    }

    // ========================
    // MÉTODOS DE ALTERAÇÃO
    // ========================

    static void AlterarMaxTokens()
    {
        Console.Write("Novo valor (ex: 50,100,200): ");
        if (int.TryParse(Console.ReadLine(), out int v)) maxTokens = v;
    }

    static void AlterarTemperature()
    {
        Console.Write("Novo valor (0 a 2): ");
        if (double.TryParse(Console.ReadLine(), out double v)) temperature = v;
    }

    static void AlterarTopP()
    {
        Console.Write("Novo valor (0 a 1): ");
        if (double.TryParse(Console.ReadLine(), out double v)) topP = v;
    }

    static void AlterarFrequencyPenalty()
    {
        Console.Write("Novo valor (-2 a 2): ");
        if (double.TryParse(Console.ReadLine(), out double v)) freqPenalty = v;
    }

    static void AlterarPresencePenalty()
    {
        Console.Write("Novo valor (-2 a 2): ");
        if (double.TryParse(Console.ReadLine(), out double v)) presencePenalty = v;
    }

    static void AlterarStopSequence()
    {
        Console.Write("Digite uma sequência (ex: ###) ou enter para remover: ");
        string? v = Console.ReadLine();
        stopSequence = string.IsNullOrWhiteSpace(v) ? null : v;
    }

    // ========================
    // MÉTODO DE PERGUNTA
    // ========================

    static async Task FazerPergunta(IChatClient client)
    {
        ChatResponse response = null!;
        Console.Clear();
        MostrarParametros();

        Console.Write("\nDigite sua pergunta: ");
        string prompt = Console.ReadLine() ?? "Explique o que é uma estrela";

        var options = new ChatOptions
        {
            MaxOutputTokens = maxTokens,
            Temperature = (float)temperature,
            TopP = (float)topP,
            FrequencyPenalty = (float)freqPenalty,
            PresencePenalty = (float)presencePenalty,
        };

        if (!string.IsNullOrWhiteSpace(stopSequence))
            options.StopSequences = new[] { stopSequence };

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nEnviando pergunta...\n");
        Console.ResetColor();

        try
        {
            response = await client.GetResponseAsync(prompt, options);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Erro ao consultar o modelo:");
            Console.WriteLine(ex.Message);
            Console.ResetColor();
            Console.ReadKey();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Assistente >>>\n");
        Console.WriteLine(response.Messages[0].Text);
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nTokens usados: prompt={response.Usage?.InputTokenCount}, resposta={response.Usage?.OutputTokenCount}");
        Console.ResetColor();

        Console.WriteLine("\nPressione qualquer tecla para continuar...");
        Console.ReadKey();
    }

    // ========================
    // INTERFACE DO MENU
    // ========================

    static void MostrarParametros()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("=== PARÂMETROS ATUAIS DO LLM ===\n");
        Console.ResetColor();

        Console.WriteLine($"1. Max Tokens.........: {maxTokens}");
        Console.WriteLine($"2. Temperature........: {temperature}");
        Console.WriteLine($"3. Top P..............: {topP}");
        Console.WriteLine($"5. Frequency Penalty..: {freqPenalty}");
        Console.WriteLine($"6. Presence Penalty...: {presencePenalty}");
        Console.WriteLine($"7. Stop Sequence......: {(stopSequence ?? "(nenhuma)")}");
    }

    static void MostrarMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n=== MENU ===");
        Console.ResetColor();

        Console.WriteLine("1 - Alterar Max Tokens");
        Console.WriteLine("2 - Alterar Temperature");
        Console.WriteLine("3 - Alterar Top-P");
        Console.WriteLine("5 - Alterar Frequency Penalty");
        Console.WriteLine("6 - Alterar Presence Penalty");
        Console.WriteLine("7 - Alterar Stop Sequence");
        Console.WriteLine("8 - Fazer pergunta ao modelo");
        Console.WriteLine("0 - Sair");
    }
}
