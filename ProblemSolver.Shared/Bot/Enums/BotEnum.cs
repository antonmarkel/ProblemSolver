using System.ComponentModel;

namespace ProblemSolver.Shared.Bot.Enums;

public enum BotEnum
{
    [Description("Meta-Llama-3_1-70B-Instruct")]
    Meta_Llama_3_1_70B_Instruct,
    [Description("Mixtral-8x7B")]
    Mixtral_8x7B,
    [Description("Mixtral-8x22B")]
    Mixtral_8x22b,
    [Description("Gemma-7b")]
    Gemma_7b
}