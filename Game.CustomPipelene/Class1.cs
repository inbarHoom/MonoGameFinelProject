namespace Game.CustomPipelene;

using System.Collections.Generic;
using System.IO;
using Microsoft.X

public List<string[]> LoadCsv(string relativePath)
{
    var rows = new List<string[]>();
    using (var stream = TitleContainer.OpenStream(relativePath))
    using (var reader = new StreamReader(stream))
    {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            rows.Add(line.Split(','));
        }
    }
    return rows;
}