﻿using System;

namespace WebLogAddin.MetaWebLogApi
{
    public class Post
    {
        public Post()
        {
            DateCreated = DateTime
                .Now;
        }

        public object PostID { get; set; }

        public DateTime DateCreated { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public string Permalink { get; set; }
        public string[] Categories { get; set; }
        public string[] Tags { get; set; }
        public CustomField[] CustomFields { get; set; }
        public Term[] Terms { get; set; }
        public string PostType { get; set; }

        public string mt_excerpt { get; set; }
        public string mt_keywords { get; set; }

        public string mt_text_more { get; set; }
        public string post_content { get; set; }

        public override string ToString()
        {
            return this.Body;
        }
    }
}