using System.Collections.Generic;
using DirigoEdgeCore.Utils;

namespace DirigoEdgeCore.Models.ViewModels
{
	public class TweetViewModel
	{
		public List<TwitterUtils.Tweet> Tweets;


		public TweetViewModel(int count)
		{
		    Tweets = TwitterUtils.GetTweets(count);
		}
	}
}