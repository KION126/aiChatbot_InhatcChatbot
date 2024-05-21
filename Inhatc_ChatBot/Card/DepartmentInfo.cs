using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class DepartmentInfo
{
    public string Name { get; set; }
    public List<string> Aliases { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }

    public static List<DepartmentInfo> LoadDepartments(string filePath)
    {
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
        var json = File.ReadAllText(fullPath);
        var departmentList = JsonConvert.DeserializeObject<DepartmentList>(json);
        return departmentList.Departments;
    }

    private class DepartmentList
    {
        public List<DepartmentInfo> Departments { get; set; }
    }
}
