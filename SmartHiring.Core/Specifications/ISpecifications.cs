﻿using SmartHiring.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartHiring.Core.Specifications
{
	public interface ISpecifications<T> where T : class
	{
		public Expression<Func<T, bool>> Criteria { get; set; }
		public List<Expression<Func<T, object>>> Includes { get; set; }
		public List<string> IncludeStrings { get; set; }

		public Expression<Func<T, object>> OrderBy { get; set; }
		public Expression<Func<T, object>> OrderByDesc { get; set; }

		public int Take { get; set; }
		public int Skip { get; set; }
		public bool IsPaginationEnabled { get; set; }
	}
}
