using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

namespace FakeNews
{
	public class HomePage
	{
		HtmlDocument index = new HtmlDocument();
		private Random rng;

		public string Build(string originalContent)
		{
			rng = new Random(new Random().Next());
			index.LoadHtml(originalContent);
			Title();		//done
			FixLinks();		//done
			Header();		//done
			MainItem();		//done
			MainColumn();	//done
			SocialLinks();  //done
			BreakingNews();	//done
			MainVideo();	//done
			SideBar();		//done
			MostPopular();	//done
			PictureRow();		//done
			NationalAndLocal();	//done
			Latest();		//done
			World();		//done
			Sport();		//done
			Business();		//done
			Money();		//done
			Entertainment();	//done
			Travel();		//done
			Technology();	//done
            SighFacebook();  //done
			using (MemoryStream resultStream = new MemoryStream()) {
				index.Save(resultStream);
				resultStream.Position = 0;
				using (StreamReader sr = new StreamReader(resultStream)) {
					return sr.ReadToEnd();
				}
			}
			
		}

		private void Title()
		{
			index.DocumentNode.SelectSingleNode("//head/title").InnerHtml = HttpUtility.HtmlEncode("FakeNews.com.au | Don't sue");
		}

		//ensure that any link on the home page goes to its real target.
		private void FixLinks()
		{
			foreach (HtmlNode link in index.DocumentNode.SelectNodes("//a[@href]")) {
				if ((!String.IsNullOrEmpty(link.Attributes["href"].Value)) && (!link.Attributes["href"].Value.StartsWith("http")))
					link.Attributes["href"].Value = "http://www.news.com.au/" + link.Attributes["href"].Value.Trim('/');
			}
		}

		//swap the header image for the fake one, add the source
		private void Header()
		{
			HtmlNode link = index.CreateElement("a");
			link.SetAttributeValue("href", "https://github.com/silpheed/FakeNews");
			HtmlNode headerLogo = index.GetElementbyId("header-logo");
			headerLogo.SetAttributeValue("style", @"background-image: url('/res/fakenews.png');");
			headerLogo.ParentNode.ReplaceChild(link, headerLogo);
			link.AppendChild(headerLogo);
		}

		private void MainItem()
		{
			const string robotDog = "http://www.youtube.com/watch?v=b2bExqhhWRI";
			HtmlNode module = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-news-home-multi-promo-main-image')]//div[contains(@class, 'module-content')]") ??
				index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-g-news-home-group-defcon-image')]//div[contains(@class, 'module-content')]") ??
				index.DocumentNode.SelectSingleNode("//div[contains(@class, 'first-image-large')]//div[contains(@class, 'module-content')]");
            if (null == module)
                return;

			//HtmlNodeCollection links = module.SelectNodes(".//div[contains(@class, 'promo-block')]//a");
			HtmlNodeCollection links = module.SelectNodes(".//a");
			int contentIndex;

			for (int i = 0; i < links.Count; i++) {
				if (i < 2) {
					links[i].SetAttributeValue("href", robotDog);
					if ((otherHeadlinesList.Count > 0) && (i == 1)) {
						contentIndex = rng.Next() % otherHeadlinesList.Count;
						links[i].InnerHtml = PrepareContent(otherHeadlinesList[contentIndex]);
						otherHeadlinesList.RemoveAt(contentIndex);
						if (null != links[i].NextSibling)
							links[i].NextSibling.Remove();
					}
				}
				else {
					if (null != links[i].NextSibling)
						links[i].NextSibling.Remove();
					links[i].Remove();
				}
			}
			//HtmlNode topBlurb = module.SelectSingleNode(".//div[contains(@class, 'promo-block')]//p[1]");
			HtmlNode topBlurb = module.SelectSingleNode(".//p[1]");
			
			if ((blurbList.Count > 0) && (null != topBlurb)) {
				contentIndex = rng.Next() % blurbList.Count;
				topBlurb.InnerHtml = PrepareContent(blurbList[contentIndex]);
				blurbList.RemoveAt(contentIndex);
			}

			//remove extra bit when special features are running
			RemoveSection("defcon-related-content");
		}

		private void MainColumn()
		{
			IList<string> silpeenUpdates = new List<string> {
                "http://twitter.com/silpeen/status/12014114984",	//what i did
				"http://twitter.com/silpeen/status/10506143520",	//what i want
				"http://twitter.com/silpeen/status/11131714048",	//iamnowdumber.com
				"http://twitter.com/silpeen/status/4800570357",		//illiterate
				"http://twitter.com/silpeen/status/11238287000",	//hey dad! strike force
			};

			HtmlNode module = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'sectionref-newscomau-top-stories')]//div[contains(@class, 'module-content')]") ??
			//if there's a "special feature" on, the layout may change
			index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-headlines')]") ??
			index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-also-in-the-news')]") ??
			index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-news-home-plmt-top-stories')]") ??
			index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-news-home-plmt-defcon-tops')]");
			if (null == module)
				return;
			
			HtmlNodeCollection headlines = module.SelectNodes(".//h4[contains(@class, 'heading')]//a");
			HtmlNodeCollection blurbs = module.SelectNodes(".//div[contains(@class, 'story-block')]//p");
			HtmlNodeCollection otherHeadlines = module.SelectNodes(".//ul[contains(@class, 'related')]//a");

			int contentIndex;
			if (null != headlines)
				foreach (HtmlNode item in headlines) {
					if (headlinesList.Count == 0)
						continue;
					contentIndex = rng.Next()%headlinesList.Count;
					item.InnerHtml = PrepareContent(headlinesList[contentIndex]);
					headlinesList.RemoveAt(contentIndex);

					if (silpeenUpdates.Count == 0)
						continue;
					item.SetAttributeValue("href", silpeenUpdates[0]);
					silpeenUpdates.RemoveAt(0);
				}
			
			if (null != blurbs)
				foreach (HtmlNode item in blurbs) {
					if (blurbList.Count == 0)
						continue;
					contentIndex = rng.Next() % blurbList.Count;
					item.InnerHtml = PrepareContent(blurbList[contentIndex]);
					blurbList.RemoveAt(contentIndex);
				}
			if (null != otherHeadlines)
				foreach (HtmlNode item in otherHeadlines) {
					if (otherHeadlinesList.Count == 0)
						continue;
					contentIndex = rng.Next() % otherHeadlinesList.Count;
					item.InnerHtml = PrepareContent(otherHeadlinesList[contentIndex]);
					otherHeadlinesList.RemoveAt(contentIndex);
				}
			
		}

		public void SocialLinks()
		{
			RemoveSection("social-links");
		}

		private void SideBar()
		{
			//leave WTF and horoscopes. no-one will be able to tell the difference.
			//RemoveSection("text-m-wtf-weird-true-freakynbsp");
			//RemoveSection("vcms-promo-widget");
			RemoveSection("text-m-extra");
			RemoveSection("video-widget-large");
			RemoveSection("text-m-blogroll");
			RemoveSection("text-m-opinion-from-the-punch");
			RemoveSection("text-m-find");
			RemoveSection("text-m-more");
			RemoveSection("multimedia-promo");
			RemoveSection("body-soul-feed");
			RemoveSection("fb_likebox");
		}

		private void BreakingNews()
		{
			//HtmlNode module = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'sectionref-breaking-news')]") ??
			HtmlNode module = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-breaking-news')]");
			if (null == module)
				return;

			HtmlNodeCollection breakingNews = module.SelectNodes(".//ul[contains(@class, 'breaking-news-list') or contains(@class, 'related')]//a");

			int contentIndex;
			if (null != breakingNews)
				foreach (HtmlNode item in breakingNews) {
					if (breakingNewsMostPopularList.Count == 0)
						continue;
					contentIndex = rng.Next() % breakingNewsMostPopularList.Count;
					item.InnerHtml = PrepareContent(breakingNewsMostPopularList[contentIndex]);
					breakingNewsMostPopularList.RemoveAt(contentIndex);
				}
		}

		private void MainVideo()
		{
			RemoveSection("video-embed");
		}

		private void MostPopular()
		{
			HtmlNode module = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'most-popular')]");
			HtmlNodeCollection mostPopular = module.SelectNodes(".//ol[1]//a");

			int contentIndex;
			if (null != mostPopular)
				foreach (HtmlNode item in mostPopular) {
					if (breakingNewsMostPopularList.Count == 0)
						continue;
					contentIndex = rng.Next() % breakingNewsMostPopularList.Count;
					item.InnerHtml = PrepareContent(breakingNewsMostPopularList[contentIndex]);
					breakingNewsMostPopularList.RemoveAt(contentIndex);
				}
		}

		private string PrepareContent(string body)
		{
			if ((body.Contains("[NAME]")) && (nameList.Count > 0)) {
				Person p = nameList[rng.Next() % nameList.Count];
				body = body.Replace("[NAME]", p.Name);
				body = body.Replace("[POS]", p.Possessive);
				body = body.Replace("[SELFPOS]", p.Ownership);
				nameList.Remove(p);
			}
			if ((body.Contains("[NAME2]")) && (nameList.Count > 0)) {
				Person p = nameList[rng.Next() % nameList.Count];
				body = body.Replace("[NAME2]", p.Name);
				nameList.Remove(p);
			}
			if ((body.Contains("[GROUP]")) && (groupList.Count > 0)) {
				Person g = groupList[rng.Next() % groupList.Count];
				body = body.Replace("[GROUP]", g.Name);
				groupList.Remove(g);
			}
			if ((body.Contains("[GROUP2]")) && (groupList.Count > 0)) {
				Person g = groupList[rng.Next() % groupList.Count];
				body = body.Replace("[GROUP2]", g.Name);
				groupList.Remove(g);
			}
			if ((body.Contains("[SUBJECT]")) && (subjectList.Count > 0)) {
				string s = subjectList[rng.Next() % subjectList.Count];
				body = body.Replace("[SUBJECT]", s);
				subjectList.Remove(s);
			}
			if ((!String.IsNullOrEmpty(body)) && (body.Substring(0, 1).ToUpperInvariant() != body.Substring(0, 1)))
				body = body.Substring(0, 1).ToUpperInvariant() + body.Substring(1);

			return body;
		}

		private void PictureRow()
		{
			HtmlNode mainModule = null;
			mainModule = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-news-home-multi-promo-scrollo')]//div[contains(@class, 'module-content')]");
			if (null == mainModule)
				return;

			HtmlNodeCollection blurbs = mainModule.SelectNodes(".//div[contains(@class, 'promo-text')]/p[1]");
			
			int contentIndex;
			if (null != blurbs)
				foreach (HtmlNode item in blurbs) {
					if (blurbList.Count == 0)
						continue;
					contentIndex = rng.Next() % blurbList.Count;
					item.InnerHtml = PrepareContent(blurbList[contentIndex]);
					blurbList.RemoveAt(contentIndex);
					while (null != item.NextSibling)
						item.NextSibling.Remove();
				}			
		}

		private void NationalAndLocal()
		{
			HtmlNode leftModule = null;
			leftModule = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-national')]//div[contains(@class, 'module-content')]");
			if (null == leftModule)
				return;

			HtmlNode topHeadline = leftModule.SelectSingleNode(".//h4[contains(@class, 'heading')]//a");
			HtmlNode topBlurb = leftModule.SelectSingleNode(".//p[contains(@class, 'standfirst')]");
			HtmlNodeCollection topOtherHeadlines = leftModule.SelectNodes("./div[contains(@class, 'story-block')]/ul[contains(@class, 'related')]//a");
			HtmlNodeCollection otherHeadlines = leftModule.SelectNodes("./ul[contains(@class, 'related')]//a");
			
			int contentIndex;
			if ((headlinesList.Count > 0) && (null != topHeadline)) {
				contentIndex = rng.Next() % headlinesList.Count;
				topHeadline.InnerHtml = PrepareContent(headlinesList[contentIndex]);
				headlinesList.RemoveAt(contentIndex);
			}
			if ((blurbList.Count > 0) && (null != topBlurb)) {
				contentIndex = rng.Next() % blurbList.Count;
				topBlurb.InnerHtml = PrepareContent(blurbList[contentIndex]);
				blurbList.RemoveAt(contentIndex);
			}
			if (null != topOtherHeadlines)
				foreach (HtmlNode item in topOtherHeadlines) {
					if (otherHeadlinesList.Count == 0)
						continue;
					contentIndex = rng.Next() % otherHeadlinesList.Count;
					item.InnerHtml = PrepareContent(otherHeadlinesList[contentIndex]);
					otherHeadlinesList.RemoveAt(contentIndex);
				}
			if (null != otherHeadlines)
				foreach (HtmlNode item in otherHeadlines) {
					if (breakingNewsMostPopularList.Count == 0)
						continue;
					contentIndex = rng.Next() % breakingNewsMostPopularList.Count;
					item.InnerHtml = PrepareContent(breakingNewsMostPopularList[contentIndex]);
					breakingNewsMostPopularList.RemoveAt(contentIndex);
				}
		}

		private void Latest()
		{
			RemoveSection("text-m-latest");
		}

		private void Business()
		{
			RemoveSection("text-g-business");
			RemoveSection("text-m-business");
		}

		private void Entertainment()
		{
			IList<string> headlinesList = new List<string> {
				"Better lives than yours",
				"Diets of the stars",
				"Look, something shiny!",
                "Hollywood puppies! Awww",
                "Name this baby bump",
                "Oscar winner marries mortal",
				"OMG! Goss! Dish dish!"
          	};

			IList<string> blurbList = new List<string> {
				"0.000005% of world's population commanding 80% of the world's media attention.",
				"Where'd all the icing sugar go? Just ask [NAME].",
				"Hollywood's best brush up on their award-winning breathing and talking skills.",
				"Citizen Kane to be remastered in 3D, 60% more explosions. McG on board.",
                "[NAME] cast as Papa Smurf in Schindler's List 2: Revenge Of The Big Lebowski.",
                "Could your child be a star? Insecure parents heed audition call."
			};

			IList<string> otherHeadlinesList = new List<string> {
				"Which TV star punched a street mime?",
				"Richard Wilkins interviews, well, who cares?",
                "Hollywood power couple spotted at car wash",
                "Actors considered role models. Yes, actors",
                "This week's latest schadenfreude",
				"Actress speaks at UN, all wars now over",
				"Stars without make-up to make you feel better",
                "Star names child after alkaline element",
				"Scientology opening the world's eyes to the truth",
			};

			HtmlNode leftModule;
			leftModule = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-news-home-plmt-ents-top-stories')]//div[contains(@class, 'module-content')]");
			if (null == leftModule)
				leftModule = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-entertainment')]//div[contains(@class, 'module-content')]");
			if (null == leftModule)
				return;
			
			HtmlNode topHeadline = leftModule.SelectSingleNode(".//h4[contains(@class, 'heading')]//a");
			HtmlNode topBlurb = leftModule.SelectSingleNode(".//p[contains(@class, 'standfirst')]");
			HtmlNodeCollection otherHeadlines = leftModule.SelectNodes(".//ul[contains(@class, 'related')]//a");

			HtmlNode rightModule;
			rightModule = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-entertainment-features')]//div[contains(@class, 'module-content')]");
			if (null == rightModule)
				rightModule = leftModule.SelectSingleNode("//ul[contains(@class, 'related')]//div[contains(@class, 'module-content')]");
			//if (null == rightModule)
			//	return;

			HtmlNodeCollection rightHeadlines = null;
			HtmlNodeCollection rightBlurbs = null;
			if (null != rightModule) {
				rightHeadlines = rightModule.SelectNodes(".//h4[contains(@class, 'heading')]//a");
				if (null == rightHeadlines)
					rightHeadlines = rightModule.SelectNodes(".//li[contains(@class, 'story')]//a");
				//if (null == rightHeadlines)
				//	return;
				rightBlurbs = rightModule.SelectNodes(".//div[contains(@class, 'promo-text')]//p");
			}

			int contentIndex;
			if ((otherHeadlinesList.Count > 0) && (null != topHeadline)) {
				contentIndex = rng.Next() % headlinesList.Count;
				topHeadline.InnerHtml = PrepareContent(headlinesList[contentIndex]);
				headlinesList.RemoveAt(contentIndex);
			}
			if ((blurbList.Count > 0) && (null != topBlurb)) {
				contentIndex = rng.Next() % blurbList.Count;
				topBlurb.InnerHtml = PrepareContent(blurbList[contentIndex]);
				blurbList.RemoveAt(contentIndex);
			}
			if (null != otherHeadlines)
				foreach (HtmlNode item in otherHeadlines) {
					if (otherHeadlinesList.Count == 0)
						continue;
					contentIndex = rng.Next() % otherHeadlinesList.Count;
					item.InnerHtml = PrepareContent(otherHeadlinesList[contentIndex]);
					otherHeadlinesList.RemoveAt(contentIndex);
				}
			if (null != rightHeadlines)
				foreach (HtmlNode item in rightHeadlines) {
					if (headlinesList.Count == 0)
						continue;
					contentIndex = rng.Next() % headlinesList.Count;
					item.InnerHtml = PrepareContent(headlinesList[contentIndex]);
					headlinesList.RemoveAt(contentIndex);
				}
			if (null != rightBlurbs)
				foreach (HtmlNode item in rightBlurbs) {
					if (blurbList.Count == 0)
						continue;
					contentIndex = rng.Next() % blurbList.Count;
					item.InnerHtml = PrepareContent(blurbList[contentIndex]);
					blurbList.RemoveAt(contentIndex);
				}
		}

		private void Travel()
		{
			RemoveSection("text-g-travel");
			RemoveSection("text-m-travel");
		}

		private void Sport()
		{
			RemoveSection("text-g-fox-sports");
		}

		private void RemoveSection(string className)
		{
			HtmlNodeCollection sections = index.DocumentNode.SelectNodes("//div[contains(@class, '" + className + "')]");
			if ((null != sections) && (sections.Count > 0))
				foreach (HtmlNode section in sections)
					section.Remove();
		}

		private void Money()
		{
			IList<string> headlinesList = new List<string>
          	{
				"Property prices go up! Invest now!",
				"Flip your house, buy another",
				"Negatively gear your property, now",
				"Best time to buy an investment property",
				"Screw others, buy more houses",
				"Investing in property good for your karma"
          	};

			IList<string> blurbList = new List<string> {
				"Lazy, undeserving young couple working only 70 hours a week each to support their one bedroom granny flat. Shrewd investors eye off opportunity.",
				"Good news for property prices, affordability drops nationwide to the benefit of everyone. Complainers rightfully branded un-Australian.",
                "Our experts explain how to cash in your super or child's education fund and invest it in property. What are you waiting for?",
                "Eating the poor is so 18th century. Relieve their burden by buying any houses that they might potentially occupy.",
				"Brighten up your day with one of our choice investment property picks. Look out for our sizzling sealed foreclosure section.",
				"Overpopulation, council regulations, greedy young couples and even foreigners to blame for high housing prices. Honest."
			};

			IList<string> otherHeadlinesList = new List<string> {
				"Naïve non-investment property owner ridiculed",
				"Investment property, investment property, investment property!",
				"Are you a property investor? You should be",
				"Our top tips for buying investment properties in bulk",
				"Snap up those prime-growth outer suburbs properties",
                "Feeling blue? Perk up by grabbing another property",
				"Investors deserving of their hard earned tax breaks",
                "How to force your kids to rent from you",
				"Negative gearing illegal in backward rest of the world"
			};
            
			HtmlNode leftModule = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-money')]//div[contains(@class, 'module-content')]");
            if (null == leftModule)
				return;

			HtmlNode topHeadline = leftModule.SelectSingleNode(".//h4[contains(@class, 'heading')]//a");
			HtmlNode topBlurb = leftModule.SelectSingleNode(".//p[contains(@class, 'standfirst')]");
			HtmlNodeCollection otherHeadlines = leftModule.SelectNodes(".//ul[contains(@class, 'related')]//a");

            HtmlNode rightModule = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-news-home-multi-promo-money')]//div[contains(@class, 'module-content')]");
            HtmlNodeCollection rightHeadlines = null;
            HtmlNodeCollection rightBlurbs = null;

            if (null != rightModule) {
                rightHeadlines = rightModule.SelectNodes(".//h4[contains(@class, 'heading')]//a");
                rightBlurbs = rightModule.SelectNodes(".//div[contains(@class, 'promo-text')]//p");
            }

			int contentIndex;
			if ((otherHeadlinesList.Count > 0) && (null != topHeadline)) {
				contentIndex = rng.Next()%headlinesList.Count;
				topHeadline.InnerHtml = headlinesList[contentIndex];
				headlinesList.RemoveAt(contentIndex);
			}
			if ((blurbList.Count > 0) && (null != topBlurb)) {
				contentIndex = rng.Next() % blurbList.Count;
				topBlurb.InnerHtml = blurbList[contentIndex];
				blurbList.RemoveAt(contentIndex);
			}
			if (null != otherHeadlines)
				foreach (HtmlNode item in otherHeadlines) {
					if (otherHeadlinesList.Count == 0)
						continue;
					contentIndex = rng.Next() % otherHeadlinesList.Count;
					item.InnerHtml = otherHeadlinesList[contentIndex];
					otherHeadlinesList.RemoveAt(contentIndex);
				}
			if (null != rightHeadlines)
				foreach (HtmlNode item in rightHeadlines) {
					if (headlinesList.Count == 0)
						continue;
					contentIndex = rng.Next() % headlinesList.Count;
					item.InnerHtml = headlinesList[contentIndex];
					headlinesList.RemoveAt(contentIndex);
				}
			if (null != rightBlurbs)
				foreach (HtmlNode item in rightBlurbs) {
					if (blurbList.Count == 0)
						continue;
					contentIndex = rng.Next() % blurbList.Count;
					item.InnerHtml = blurbList[contentIndex];
					blurbList.RemoveAt(contentIndex);
				}
		}
        
		private void Technology()
		{
			//nothing but iphones and facebook!
			HtmlNode leftModule =
				index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-news-home-plmt-tech-top-stories')]//div[contains(@class, 'module-content')]")
				?? index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-technology')]//div[contains(@class, 'module-content')]")
				;
			
			HtmlNode rightModule = null;
			rightModule = index.DocumentNode.SelectSingleNode("//div[contains(@class, 'text-m-industry-news-from-australianit')]//div[contains(@class, 'module-content')]");

			HtmlNode topHeadlineLeft = null;
			HtmlNode topBlurbLeft = null;
			HtmlNodeCollection otherHeadlinesLeft = null;
			HtmlNode topHeadlineRight = null;
			HtmlNode topBlurbRight = null;
			HtmlNodeCollection otherHeadlinesRight = null;
			IList<string> iPhoneFacebookHeadline;
			IList<string> iPhoneFacebookOther = new List<string>();

			if (null != leftModule) {
				topHeadlineLeft = leftModule.SelectSingleNode(".//h4[contains(@class, 'heading')]//a");
				topBlurbLeft = leftModule.SelectSingleNode(".//p[contains(@class, 'standfirst')]");
				otherHeadlinesLeft = leftModule.SelectNodes(".//ul[contains(@class, 'related')]//a");
			}
			if (null != rightModule) {
				topHeadlineRight = rightModule.SelectSingleNode(".//h4[contains(@class, 'heading')]//a");
				topBlurbRight = rightModule.SelectSingleNode(".//p[contains(@class, 'standfirst')]");
				otherHeadlinesRight = rightModule.SelectNodes(".//ul[contains(@class, 'related')]//a");
			}
			iPhoneFacebookHeadline = GetiPhoneFacebook(38,2);

			int otherHeadlinesLeftCount = (null != otherHeadlinesLeft) ? otherHeadlinesLeft.Count : 0;
			int otherHeadlinesRightCount = (null != otherHeadlinesRight) ? otherHeadlinesRight.Count : 0;
			iPhoneFacebookOther = GetiPhoneFacebook(45, otherHeadlinesLeftCount + otherHeadlinesRightCount);
			
			if (null != topHeadlineLeft)
				topHeadlineLeft.InnerHtml = iPhoneFacebookHeadline[0];
			if (null != topHeadlineRight)
				topHeadlineRight.InnerHtml = iPhoneFacebookHeadline[1];
			IList<string> iPhoneFacebookBlurbs = GetiPhoneFacebook(110, 2);
			if (null != topBlurbLeft)
				topBlurbLeft.InnerHtml = iPhoneFacebookBlurbs[0];
			if (null != topBlurbRight)
				topBlurbRight.InnerHtml = iPhoneFacebookBlurbs[1];
			if (null != otherHeadlinesLeft)
				for (int i = 0; i < otherHeadlinesLeft.Count; i++)
					if (i < iPhoneFacebookOther.Count)
						otherHeadlinesLeft[i].InnerHtml = iPhoneFacebookOther[i];
			if ((null != otherHeadlinesLeft) && (null != otherHeadlinesRight))
				for (int i = 0; i < otherHeadlinesRight.Count; i++)
					if (i < iPhoneFacebookOther.Count)
						otherHeadlinesRight[i].InnerHtml = iPhoneFacebookOther[i + otherHeadlinesLeftCount];
		}

		//this surprisingly complex function provides:
		//1) a list of random headlines of either "iPhone" or "Facebook" repeated
		//2) equal quantities of both
		//3) ensurance that lines aren't too long
		//4) a piece of random punctuation in a random position, most of the time
		private IList<string> GetiPhoneFacebook(int chars, int instances)
		{
			IList<string> output = new List<string>();

			int maxInstances = instances / 2;
			int iPhones = 0, facebooks = 0;

			if (instances % 2 > 0) {
				if (rng.Next() % 2 == 0)
					iPhones = -1;
				else
					facebooks = -1;
			}
            
            for (int i = 0; i < instances; i++) {
				//rng.Next() % 2 seems more random than rng.Next(0, 2), without going into a cryptographically strong algorithm
				int type = rng.Next() % 2;
				
				if (((type == 0) && (iPhones < maxInstances)) || ((type == 1) && (facebooks >= maxInstances))) {
					output.Add(BuildTechnologyHeadline("iPhone", chars));
					iPhones++;
				}
				else {
					output.Add(BuildTechnologyHeadline("Facebook", chars));
					facebooks++;
				}
			}
			return output;
		}

		private string BuildTechnologyHeadline(string word, int totalChars)
		{
			int numWords = totalChars / (word.Length + 1);
			string line = new String(' ', numWords).Replace(" ", word + " ").Trim();

			string[] punctuation = new[] { "!", ",", ";", "...", " &" };
			int puncType = rng.Next() % (punctuation.Length + 1);
			
			if (puncType < punctuation.Length) {
				//"!" is allowed to be at the end of a line, the others are not
				int puncPos = rng.Next() % (puncType == 0 ? numWords : (numWords - 1));
				string[] words = line.Split(' ');
				words[puncPos] = words[puncPos] + punctuation[puncType];
				line = string.Empty;
				foreach (string w in words)
					line += w + " ";
			}
			return line;
		}

		private void World()
		{
			RemoveSection("text-g-world");
			RemoveSection("text-m-world");
		}

        private void SighFacebook()
        {
            RemoveSection("fb_recommendations");
        }

		IList<Person> nameList = new List<Person> {
				new Person { Name = "Madonna", Gender = Gender.Female },
				new Person { Name = "Paris Hilton", Gender = Gender.Female },
				new Person { Name = "Corey Worthington", Gender = Gender.Male, NamePrefix = "serial party boy" },
				new Person { Name = "R-Patz", Gender = Gender.Male },
				new Person { Name = "Lady Gaga", Gender = Gender.Female },
				new Person { Name = "K-Rudd", Gender = Gender.Male },
				new Person { Name = "Sheik Hilaly", Gender = Gender.Male },
				new Person { Name = "Ashton Kutcher", Gender = Gender.Male },
				new Person { Name = "Lindsay Lohan", Gender = Gender.Female },
				new Person { Name = "J Lo", Gender = Gender.Female },
				new Person { Name = "Germaine Greer", Gender = Gender.Female },
				new Person { Name = "James Packer", Gender = Gender.Male },
				new Person { Name = "Suri Cruise", Gender = Gender.Female },
				new Person { Name = "Megan Gale", Gender = Gender.Female },
				new Person { Name = "Lara Bingle", Gender = Gender.Female },
				new Person { Name = "Jennifer Hawkins", Gender = Gender.Female },
				new Person { Name = "Delta Goodrem", Gender = Gender.Female },
				new Person { Name = "Britney Spears", Gender = Gender.Female },
				new Person { Name = "K-Fed", Gender = Gender.Male },
				new Person { Name = "Sarah Palin", Gender = Gender.Female },
				new Person { Name = "Kyle Sandilands", Gender = Gender.Male },
				new Person { Name = "Ian \"Dicko\" Dickson", Gender = Gender.Male },
                new Person { Name = "Jessica Watson", Gender = Gender.Female, NamePrefix = "solo sailor" },
				new Person { Name = "Gordon Ramsay", Gender = Gender.Male },
				new Person { Name = "a fat guy on a plane", Gender = Gender.Male },
				new Person { Name = "former PM", Gender = Gender.Male },
				new Person { Name = "Australian Idol winner", Gender = Gender.Male },
				new Person { Name = "Barack Obama", Gender = Gender.Male }
			};

		IList<Person> groupList = new List<Person> {
				new Person { Name = "TomKat", Gender = Gender.Group },
				new Person { Name = "Brangelina", Gender = Gender.Group },
				new Person { Name = "Lleyton and Bec", Gender = Gender.Group },
				new Person { Name = "Kochie and Mel", Gender = Gender.Group },
				new Person { Name = "Keith Urban and our Nic", Gender = Gender.Group },
				new Person { Name = "the paparazzi", Gender = Gender.Group },
				new Person { Name = "our Diggers", Gender = Gender.Group },
				new Person { Name = "working families", Gender = Gender.Group },
				new Person { Name = "those fatcat pollies", Gender = Gender.Group },
				new Person { Name = "twitter celebrities", Gender = Gender.Group },
				new Person { Name = "parents", Gender = Gender.Group },
				new Person { Name = "war orphans", Gender = Gender.Group },
				new Person { Name = "PETA", Gender = Gender.Group }
			};
        
		IList<string> breakingNewsMostPopularList = new List<string> {
				"[NAME] in nip-slip drama",
				"[NAME] eats [POS] own head",
				"Puff piece about a YouTube clip",
				"Breast feeding article, possible boob pics",
				"Asylum seekers queuejumping the floodgates",
				"Outrage as concerns spark crisis talks",
				"K-Rudd drops F-bomb on all you N-words",
				"Disagreement elevated to scandal; on way to crisis",
				"[GROUP] sue [GROUP2]",
				"Brave celebrity raises awareness of illness",
				"Survey says Sydney better than Melbourne",
				"Survey says Melbourne better than Sydney",
				"Rolling plants down stairs new web sensation",
				"[NAME][SELFPOS] [SUBJECT] crisis",
				"Fat, lazy adults concerned about fat, lazy kids",
				"All work and no play makes Jack a dull boy",
				"Four month old foetus releases new album",
				"Calls for inquiry into that sort of thing",
				"News videos now 110% more inane",
				"Government minister caught having lunch",
				"Article sourced from News Of The World",
				"Open comment thread for illiterate and inbred",
				"Randoms turn up at Davo's place, LOL",
				"WHO eradicates typhoburcosomething",
				"Missing white woman still gone, others too",
				"[NAME] gets ink done",
				"Cosy prisoners get own bed, sink",
				"[NAME] in new reality show",
				"\"Staff Writers\" win journalism award",
				"Fallout continues from \"Pancakegate\"",
                "Interest rates rise, Opposition calls for inquiry",
				"Interest rates fall, Opposition calls for inquiry",
				"Those bastard banks make a profit",
                "Drug user allowed to vote, own property",
			};

		IList<string> headlinesList = new List<string> {
				"Fisher Price enabling toddlers' \"sexting\"",
                "Father eats wife, child. Dog spared",
                "[NAME] hits a cyclist",
                "MasterChef episode happens",
				"[NAME] orders a sandwich; egg salad",
				"[NAME] sure does like kittens",
				"[NAME] piles on the pounds",
				"[NAME] burns [POS] tongue",
				"[NAME][SELFPOS] photo-op with [GROUP]",
				"Something about flash crowds",
				"[GROUP] should know better",
                "Same Gen Y beat-up from last week"
          	};

		IList<string> blurbList = new List<string> {
				"Thousands flock to stain on wall shaped like [NAME].",
                "[NAME] holds auditions for new BFF; [NAME2] a hot favourite.",
                "You suckers will be paying to read this crap soon. Seriously.",
                "[NAME] goes shopping. Media stirred into a frenzy!",
                "[NAME] breaks [POS] silence on [SUBJECT].",
                "[NAME] holds crisis talks about [SUBJECT]. UN passes resolution.",
                "[NAME] speaks out over [SUBJECT].",
                "[NAME] caught fondling [NAME2].",
                "Meddling kids and their dog apprehend fairground rapist. Revealed to be [NAME].",
				"[NAME] placed in custody and questioned over the death of Laura Palmer.",
                "Could [NAME] be in rehab? Celebrity blogger Perez Hilton says \"Yuh huh\".",
                "[NAME] gives fashion tips to [GROUP], with sexy results.",
				"Outrage over Australian movie containing cuss words financed by hundreds of taxpayer dollars.",
				"Child advocate group outraged at their lack of media exposure. Press conference held.",
                "Diners dismayed as several delicious bacon bits contaminated in salad bar mayonnaise spill. Experts examining partition failure.",
                "Climate change deniers claim that global warming which is not happening is not caused by man.",
                "Potentially reputable article salvaged by mentioning tenuous link to breasts in headline.",
                "Sex offender caught living within 300km of school. [NAME] shares [POS] outrage.",
				"[GROUP] protest over [NAME][SELFPOS] [SUBJECT] comments."
			};
			
		//No bigger than this--------------------------------
		IList<string> otherHeadlinesList = new List<string> {
				"<strong>GFC:</strong> What our celebs think",
				"<strong>Pics:</strong> Latest wardrobe malfunctions",
                "<strong>Werewolves:</strong> Are you prepared?",
                "<strong>Chocolate:</strong> Good for you this week",
                "<strong>LOL:</strong> Boffins cure cancer",
                "<strong>YouTube:</strong> Cats doing people things",
                "<strong>Bird flu:</strong> We ask Twitter celebs",
				"<strong>Survey:</strong> Boxers or briefs?",
                "<strong>Annoying video:</strong> Can't pause/skip",
                "<strong>Gallery:</strong> Pics ripped from blogs",
                "<strong>UFOs:</strong> Top secret blurry pics",
                "<strong>Logies:</strong> Why you should care",
				"<strong>Poll:</strong> Warnie for PM?"
			};

		private IList<string> subjectList = new List<string> {
        		"camel racing",
        		"sausages",
        		"nude protesting",
        		"African countries",
        		"competitive archery",
        		"parallel parking"
        	};

		private struct Person
		{
			public string Name;
			public Gender Gender;
			public string NamePrefix;
			public string Possessive
			{
				get {
					if (Gender == Gender.Male)
						return "his";
					if (Gender == Gender.Female)
						return "her";
					return "their";
				}

			}
            public string Ownership
			{
				get {
					if (String.IsNullOrEmpty(Name))
						return String.Empty;
					if (Name[Name.Length - 1].Equals('s'))
						return "'";
					return "'s";
				}
				
			}
			//e.g. Dave hits someone, Clowns hit someone
			public string ActionSuffix
			{
				get {
					if (Gender == Gender.Group)
						return "";
					return "s";
				}
			}
			
		}

		private enum Gender
		{ Group, Male, Female }

	}
}