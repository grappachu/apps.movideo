using System;
using System.IO;
using System.Linq;
using Grappachu.Movideo.Core.Models;

namespace Grappachu.Movideo.Core.Components.MediaOrganizer
{
    internal static class TemplateConverter
    {

        public static bool IsValid(string template)
        {
            var val = template;

            foreach (var token in Tokens.List)
                val = val.Replace(token, string.Empty);

            return !val.Contains("%");
        }

        public static string Convert(string template, FileInfo item, Movie movie)
        {
            var fren = template;
            fren = fren.Replace(Tokens.Title, movie.Title);
            fren = fren.Replace(Tokens.Year, movie.Year?.ToString() ?? string.Empty);
            fren = fren.Replace(Tokens.Collection, movie.Collection);
            fren = fren.Replace(Tokens.Extension, item.Extension.TrimStart('.'));
            fren = fren.Replace(Tokens.Genre, movie.Genres.FirstOrDefault()?.Name);
            fren = fren.Replace(Tokens.AllGenres, string.Join(",", movie.Genres.Select(x => x.Name)));
            fren = fren.Replace(Tokens.Rating, movie.VoteAverage.ToString("F1"));
            fren = fren.Replace(Tokens.Popularity, movie.Popularity.ToString("F2"));

            while (fren.Contains(@"\\"))
            {
                fren = fren.Replace(@"\\", @"\");
            }

            var frenamed = CleanFileName(fren);

            return frenamed;
        }


        private static string CleanFileName(string fileName)
        {
            var pathTokens = fileName.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < pathTokens.Length; i++)
            {
                var token = pathTokens[i];
                pathTokens[i] = Path.GetInvalidFileNameChars()
                    .Aggregate(token, (current, c) => current.Replace(c.ToString(), string.Empty));
            }
            return string.Join("\\", pathTokens);
        }
    }
}