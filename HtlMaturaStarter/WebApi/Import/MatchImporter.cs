using DataAccess;

public class MatchImporter
{
    private readonly ApplicationDataContext _db;

    public MatchImporter(ApplicationDataContext db)
    {
        Console.WriteLine("MatchImporter created with db context " + db);
        _db = db;
    }

    public void ImportFolder(string rootFolder)
    {
        Console.WriteLine($"Importing folder: {rootFolder}");
        foreach (var dir in Directory.GetDirectories(rootFolder))
        {
            Console.WriteLine($"Importing folder: {dir}");
            ImportMatch(dir);
        }
    }


    public void ImportMatch(string folder)
    {
        var parts = Path.GetFileName(folder).Split('-');
            throw new NotImplementedException();
    }

    private void ParseLineup(Match match, string file)
    {
        var lines = File.ReadAllLines(file);
        throw new NotImplementedException();
    }

    private void ParseEvents(Match match, string file)
    {
        throw new NotImplementedException();
    }

}
