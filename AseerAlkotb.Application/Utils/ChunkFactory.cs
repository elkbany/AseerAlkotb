﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AseerAlkotb.Domain.Entites.Models;

namespace AseerAlkotb.Application.Utils
{
    public static class ChunkFactory
    {
        public const int MIN_DESC_CHARS = 160;
        public const int MIN_BIO_CHARS = 80;

        public static List<(string Type, string Content)> BuildBookChunks(Book b)
        {
            var chunks = new List<(string, string)>();

            if (!string.IsNullOrWhiteSpace(b.Title))
                chunks.Add(("title", b.Title));

            if (!string.IsNullOrWhiteSpace(b.Description))
                chunks.Add(("description", b.Description));

            if (!string.IsNullOrWhiteSpace(b.Author?.Name))
                chunks.Add(("author", b.Author!.Name));

            var cats = string.Join(", ", b.Categories?.Select(c => c.Name) ?? Enumerable.Empty<string>());
            if (!string.IsNullOrWhiteSpace(cats))
                chunks.Add(("category", cats));

            // Always include author bio if available (previously was conditional)
            if (!string.IsNullOrWhiteSpace(b.Author?.Bio))
                chunks.Add(("author_bio", b.Author!.Bio!));

            // Add publisher info if available
            if (!string.IsNullOrWhiteSpace(b.Publisher?.Name))
                chunks.Add(("publisher", b.Publisher!.Name));

            if (!string.IsNullOrWhiteSpace(b.Publisher?.Description))
                chunks.Add(("publisher_bio", b.Publisher!.Description));

            return chunks;
        }

        public static bool HasRichDescription(Book b) =>
            !string.IsNullOrWhiteSpace(b.Description) && b.Description!.Trim().Length >= MIN_DESC_CHARS;

        public static bool HasGoodBio(string? bio) =>
            !string.IsNullOrWhiteSpace(bio) && bio!.Trim().Length >= MIN_BIO_CHARS;
    }
}