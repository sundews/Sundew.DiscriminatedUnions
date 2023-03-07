namespace Sundew.Quantities.Generator;

using Sundew.Generator;
using System.Threading.Tasks;
using Sundew.Generator.Code;
using Sundew.Generator.Input;
using Sundew.Generator.Output;

public static class Program
{
    public static Task Main()
    {
        return GeneratorFacade.RunAsync(new Setup
        {
            ModelSetup = new FolderModelSetup
            {
                Folder = "../../../../Sundew.DiscriminatedUnions.Shared/Api",
                Provider = new TextModelProvider(),
                FilesSearchPattern = "*.cs",
            },
            GeneratorSetups = new[]
            {
                new GeneratorSetup
                {
                    Generator = new PublicApiGenerator(),
                },
            },
            WriterSetups = new[]
            {
                new FileWriterSetup("../../../../Sundew.DiscriminatedUnions/Sundew.DiscriminatedUnions.csproj")
                {
                    Writer = new ProjectTextFileWriter(),
                    FileExtension = ".cs",
                    FileNameSuffix = ".g",
                    Folder = ".generated",
                },
            }
        });
    }
}