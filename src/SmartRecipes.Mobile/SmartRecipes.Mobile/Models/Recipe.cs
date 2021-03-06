﻿using System;
using FuncSharp;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public class Recipe : Entity, IRecipe
    {
        private const string DefaultImageUrl = "https://cdn1.iconfinder.com/data/icons/food-solid-icons-volume-4-1/128/171-512.png";

        private Recipe(Guid id, Guid ownerId, string name, Uri imageUrl, int personCount, string text) : base(id)
        {
            Name = name;
            ImageUrl = imageUrl;
            OwnerId = ownerId;
            PersonCount = personCount;
            Text = text;
        }

        public Recipe() : base(Guid.Empty) { /* for sqllite */ }

        public Guid OwnerId { get; set; }

        public string Name { get; set; }

        public string _ImageUrl { get; set; }
        [Ignore]
        public Uri ImageUrl
        {
            get { return new Uri(_ImageUrl); }
            set { _ImageUrl = value.AbsoluteUri; }
        }

        public int PersonCount { get; set; }

        public string Text { get; set; }

        // Combinators
        
        public static Recipe Create(Guid id, Guid ownerId, string name, Uri imageUrl, int personCount, string text)
        {
            return new Recipe(id, ownerId, name, imageUrl, personCount, text);
        }
        
        public static IRecipe Create(IAccount owner, string name, IOption<Uri> imageUrl, int personCount, string text)
        {
            return Create(Guid.NewGuid(), owner.Id, name, imageUrl.GetOrElse(new Uri(DefaultImageUrl)), personCount, text);
        }
    }
}
