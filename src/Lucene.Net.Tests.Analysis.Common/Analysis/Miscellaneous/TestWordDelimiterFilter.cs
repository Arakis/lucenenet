﻿using System;
using System.Collections.Generic;

/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace org.apache.lucene.analysis.miscellaneous
{

	using org.apache.lucene.analysis;
	using TokenStreamComponents = org.apache.lucene.analysis.Analyzer.TokenStreamComponents;
	using KeywordTokenizer = org.apache.lucene.analysis.core.KeywordTokenizer;
	using StopFilter = org.apache.lucene.analysis.core.StopFilter;
	using CzechStemFilter = org.apache.lucene.analysis.cz.CzechStemFilter;
	using StandardAnalyzer = org.apache.lucene.analysis.standard.StandardAnalyzer;
	using PositionIncrementAttribute = org.apache.lucene.analysis.tokenattributes.PositionIncrementAttribute;
	using CharTermAttribute = org.apache.lucene.analysis.tokenattributes.CharTermAttribute;
	using CharArraySet = org.apache.lucene.analysis.util.CharArraySet;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.apache.lucene.analysis.miscellaneous.WordDelimiterFilter.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.apache.lucene.analysis.miscellaneous.WordDelimiterIterator.DEFAULT_WORD_DELIM_TABLE;

	/// <summary>
	/// New WordDelimiterFilter tests... most of the tests are in ConvertedLegacyTest
	/// TODO: should explicitly test things like protWords and not rely on
	/// the factory tests in Solr.
	/// </summary>
	public class TestWordDelimiterFilter : BaseTokenStreamTestCase
	{

	  /// <summary>
	  ///*
	  /// public void testPerformance() throws IOException {
	  ///  String s = "now is the time-for all good men to come to-the aid of their country.";
	  ///  Token tok = new Token();
	  ///  long start = System.currentTimeMillis();
	  ///  int ret=0;
	  ///  for (int i=0; i<1000000; i++) {
	  ///    StringReader r = new StringReader(s);
	  ///    TokenStream ts = new WhitespaceTokenizer(r);
	  ///    ts = new WordDelimiterFilter(ts, 1,1,1,1,0);
	  /// 
	  ///    while (ts.next(tok) != null) ret++;
	  ///  }
	  /// 
	  ///  System.out.println("ret="+ret+" time="+(System.currentTimeMillis()-start));
	  /// }
	  /// **
	  /// </summary>

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOffsets() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testOffsets()
	  {
		int flags = GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | CATENATE_ALL | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;
		// test that subwords and catenated subwords have
		// the correct offsets.
		WordDelimiterFilter wdf = new WordDelimiterFilter(TEST_VERSION_CURRENT, new SingleTokenTokenStream(new Token("foo-bar", 5, 12)), DEFAULT_WORD_DELIM_TABLE, flags, null);

		assertTokenStreamContents(wdf, new string[] {"foo", "foobar", "bar"}, new int[] {5, 5, 9}, new int[] {8, 12, 12});

		wdf = new WordDelimiterFilter(TEST_VERSION_CURRENT, new SingleTokenTokenStream(new Token("foo-bar", 5, 6)), DEFAULT_WORD_DELIM_TABLE, flags, null);

		assertTokenStreamContents(wdf, new string[] {"foo", "bar", "foobar"}, new int[] {5, 5, 5}, new int[] {6, 6, 6});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOffsetChange() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testOffsetChange()
	  {
		int flags = GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | CATENATE_ALL | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;
		WordDelimiterFilter wdf = new WordDelimiterFilter(TEST_VERSION_CURRENT, new SingleTokenTokenStream(new Token("übelkeit)", 7, 16)), DEFAULT_WORD_DELIM_TABLE, flags, null);

		assertTokenStreamContents(wdf, new string[] {"übelkeit"}, new int[] {7}, new int[] {15});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOffsetChange2() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testOffsetChange2()
	  {
		int flags = GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | CATENATE_ALL | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;
		WordDelimiterFilter wdf = new WordDelimiterFilter(TEST_VERSION_CURRENT, new SingleTokenTokenStream(new Token("(übelkeit", 7, 17)), DEFAULT_WORD_DELIM_TABLE, flags, null);

		assertTokenStreamContents(wdf, new string[] {"übelkeit"}, new int[] {8}, new int[] {17});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOffsetChange3() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testOffsetChange3()
	  {
		int flags = GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | CATENATE_ALL | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;
		WordDelimiterFilter wdf = new WordDelimiterFilter(TEST_VERSION_CURRENT, new SingleTokenTokenStream(new Token("(übelkeit", 7, 16)), DEFAULT_WORD_DELIM_TABLE, flags, null);

		assertTokenStreamContents(wdf, new string[] {"übelkeit"}, new int[] {8}, new int[] {16});
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOffsetChange4() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testOffsetChange4()
	  {
		int flags = GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | CATENATE_ALL | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;
		WordDelimiterFilter wdf = new WordDelimiterFilter(TEST_VERSION_CURRENT, new SingleTokenTokenStream(new Token("(foo,bar)", 7, 16)), DEFAULT_WORD_DELIM_TABLE, flags, null);

		assertTokenStreamContents(wdf, new string[] {"foo", "foobar", "bar"}, new int[] {8, 8, 12}, new int[] {11, 15, 15});
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void doSplit(final String input, String... output) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void doSplit(string input, params string[] output)
	  {
		int flags = GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;
		WordDelimiterFilter wdf = new WordDelimiterFilter(TEST_VERSION_CURRENT, new MockTokenizer(new StringReader(input), MockTokenizer.KEYWORD, false), WordDelimiterIterator.DEFAULT_WORD_DELIM_TABLE, flags, null);

		assertTokenStreamContents(wdf, output);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSplits() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSplits()
	  {
		doSplit("basic-split","basic","split");
		doSplit("camelCase","camel","Case");

		// non-space marking symbol shouldn't cause split
		// this is an example in Thai    
		doSplit("\u0e1a\u0e49\u0e32\u0e19","\u0e1a\u0e49\u0e32\u0e19");
		// possessive followed by delimiter
		doSplit("test's'", "test");

		// some russian upper and lowercase
		doSplit("Роберт", "Роберт");
		// now cause a split (russian camelCase)
		doSplit("РобЕрт", "Роб", "Ерт");

		// a composed titlecase character, don't split
		doSplit("aǅungla", "aǅungla");

		// a modifier letter, don't split
		doSplit("ســـــــــــــــــلام", "ســـــــــــــــــلام");

		// enclosing mark, don't split
		doSplit("test⃝", "test⃝");

		// combining spacing mark (the virama), don't split
		doSplit("हिन्दी", "हिन्दी");

		// don't split non-ascii digits
		doSplit("١٢٣٤", "١٢٣٤");

		// don't split supplementaries into unpaired surrogates
		doSplit("𠀀𠀀", "𠀀𠀀");
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void doSplitPossessive(int stemPossessive, final String input, final String... output) throws Exception
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  public virtual void doSplitPossessive(int stemPossessive, string input, params string[] output)
	  {
		int flags = GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS;
		flags |= (stemPossessive == 1) ? STEM_ENGLISH_POSSESSIVE : 0;
		WordDelimiterFilter wdf = new WordDelimiterFilter(TEST_VERSION_CURRENT, new MockTokenizer(new StringReader(input), MockTokenizer.KEYWORD, false), flags, null);

		assertTokenStreamContents(wdf, output);
	  }

	  /*
	   * Test option that allows disabling the special "'s" stemming, instead treating the single quote like other delimiters. 
	   */
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPossessives() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPossessives()
	  {
		doSplitPossessive(1, "ra's", "ra");
		doSplitPossessive(0, "ra's", "ra", "s");
	  }

	  /*
	   * Set a large position increment gap of 10 if the token is "largegap" or "/"
	   */
	  private sealed class LargePosIncTokenFilter : TokenFilter
	  {
		  private readonly TestWordDelimiterFilter outerInstance;

		internal CharTermAttribute termAtt = addAttribute(typeof(CharTermAttribute));
		internal PositionIncrementAttribute posIncAtt = addAttribute(typeof(PositionIncrementAttribute));

		protected internal LargePosIncTokenFilter(TestWordDelimiterFilter outerInstance, TokenStream input) : base(input)
		{
			this.outerInstance = outerInstance;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public boolean incrementToken() throws java.io.IOException
		public override bool incrementToken()
		{
		  if (input.incrementToken())
		  {
			if (termAtt.ToString().Equals("largegap") || termAtt.ToString().Equals("/"))
			{
			  posIncAtt.PositionIncrement = 10;
			}
			return true;
		  }
		  else
		  {
			return false;
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPositionIncrements() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testPositionIncrements()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int flags = GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | CATENATE_ALL | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;
		int flags = GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | CATENATE_ALL | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.lucene.analysis.util.CharArraySet protWords = new org.apache.lucene.analysis.util.CharArraySet(TEST_VERSION_CURRENT, new HashSet<>(Arrays.asList("NUTCH")), false);
		CharArraySet protWords = new CharArraySet(TEST_VERSION_CURRENT, new HashSet<>("NUTCH"), false);

		/* analyzer that uses whitespace + wdf */
		Analyzer a = new AnalyzerAnonymousInnerClassHelper(this, flags, protWords);

		/* in this case, works as expected. */
		assertAnalyzesTo(a, "LUCENE / SOLR", new string[] {"LUCENE", "SOLR"}, new int[] {0, 9}, new int[] {6, 13}, new int[] {1, 1});

		/* only in this case, posInc of 2 ?! */
		assertAnalyzesTo(a, "LUCENE / solR", new string[] {"LUCENE", "sol", "solR", "R"}, new int[] {0, 9, 9, 12}, new int[] {6, 12, 13, 13}, new int[] {1, 1, 0, 1});

		assertAnalyzesTo(a, "LUCENE / NUTCH SOLR", new string[] {"LUCENE", "NUTCH", "SOLR"}, new int[] {0, 9, 15}, new int[] {6, 14, 19}, new int[] {1, 1, 1});

		/* analyzer that will consume tokens with large position increments */
		Analyzer a2 = new AnalyzerAnonymousInnerClassHelper2(this, flags, protWords);

		/* increment of "largegap" is preserved */
		assertAnalyzesTo(a2, "LUCENE largegap SOLR", new string[] {"LUCENE", "largegap", "SOLR"}, new int[] {0, 7, 16}, new int[] {6, 15, 20}, new int[] {1, 10, 1});

		/* the "/" had a position increment of 10, where did it go?!?!! */
		assertAnalyzesTo(a2, "LUCENE / SOLR", new string[] {"LUCENE", "SOLR"}, new int[] {0, 9}, new int[] {6, 13}, new int[] {1, 11});

		/* in this case, the increment of 10 from the "/" is carried over */
		assertAnalyzesTo(a2, "LUCENE / solR", new string[] {"LUCENE", "sol", "solR", "R"}, new int[] {0, 9, 9, 12}, new int[] {6, 12, 13, 13}, new int[] {1, 11, 0, 1});

		assertAnalyzesTo(a2, "LUCENE / NUTCH SOLR", new string[] {"LUCENE", "NUTCH", "SOLR"}, new int[] {0, 9, 15}, new int[] {6, 14, 19}, new int[] {1, 11, 1});

		Analyzer a3 = new AnalyzerAnonymousInnerClassHelper3(this, flags, protWords);

		assertAnalyzesTo(a3, "lucene.solr", new string[] {"lucene", "lucenesolr", "solr"}, new int[] {0, 0, 7}, new int[] {6, 11, 11}, new int[] {1, 0, 1});

		/* the stopword should add a gap here */
		assertAnalyzesTo(a3, "the lucene.solr", new string[] {"lucene", "lucenesolr", "solr"}, new int[] {4, 4, 11}, new int[] {10, 15, 15}, new int[] {2, 0, 1});
	  }

	  private class AnalyzerAnonymousInnerClassHelper : Analyzer
	  {
		  private readonly TestWordDelimiterFilter outerInstance;

		  private int flags;
		  private CharArraySet protWords;

		  public AnalyzerAnonymousInnerClassHelper(TestWordDelimiterFilter outerInstance, int flags, CharArraySet protWords)
		  {
			  this.outerInstance = outerInstance;
			  this.flags = flags;
			  this.protWords = protWords;
		  }

		  public override TokenStreamComponents createComponents(string field, Reader reader)
		  {
			Tokenizer tokenizer = new MockTokenizer(reader, MockTokenizer.WHITESPACE, false);
			return new TokenStreamComponents(tokenizer, new WordDelimiterFilter(TEST_VERSION_CURRENT, tokenizer, flags, protWords));
		  }
	  }

	  private class AnalyzerAnonymousInnerClassHelper2 : Analyzer
	  {
		  private readonly TestWordDelimiterFilter outerInstance;

		  private int flags;
		  private CharArraySet protWords;

		  public AnalyzerAnonymousInnerClassHelper2(TestWordDelimiterFilter outerInstance, int flags, CharArraySet protWords)
		  {
			  this.outerInstance = outerInstance;
			  this.flags = flags;
			  this.protWords = protWords;
		  }

		  public override TokenStreamComponents createComponents(string field, Reader reader)
		  {
			Tokenizer tokenizer = new MockTokenizer(reader, MockTokenizer.WHITESPACE, false);
			return new TokenStreamComponents(tokenizer, new WordDelimiterFilter(TEST_VERSION_CURRENT, new LargePosIncTokenFilter(outerInstance, tokenizer), flags, protWords));
		  }
	  }

	  private class AnalyzerAnonymousInnerClassHelper3 : Analyzer
	  {
		  private readonly TestWordDelimiterFilter outerInstance;

		  private int flags;
		  private CharArraySet protWords;

		  public AnalyzerAnonymousInnerClassHelper3(TestWordDelimiterFilter outerInstance, int flags, CharArraySet protWords)
		  {
			  this.outerInstance = outerInstance;
			  this.flags = flags;
			  this.protWords = protWords;
		  }

		  public override TokenStreamComponents createComponents(string field, Reader reader)
		  {
			Tokenizer tokenizer = new MockTokenizer(reader, MockTokenizer.WHITESPACE, false);
			StopFilter filter = new StopFilter(TEST_VERSION_CURRENT, tokenizer, StandardAnalyzer.STOP_WORDS_SET);
			return new TokenStreamComponents(tokenizer, new WordDelimiterFilter(TEST_VERSION_CURRENT, filter, flags, protWords));
		  }
	  }

	  /// <summary>
	  /// concat numbers + words + all </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testLotsOfConcatenating() throws Exception
	  public virtual void testLotsOfConcatenating()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int flags = GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | CATENATE_WORDS | CATENATE_NUMBERS | CATENATE_ALL | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;
		int flags = GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | CATENATE_WORDS | CATENATE_NUMBERS | CATENATE_ALL | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;

		/* analyzer that uses whitespace + wdf */
		Analyzer a = new AnalyzerAnonymousInnerClassHelper4(this, flags);

		assertAnalyzesTo(a, "abc-def-123-456", new string[] {"abc", "abcdef", "abcdef123456", "def", "123", "123456", "456"}, new int[] {0, 0, 0, 4, 8, 8, 12}, new int[] {3, 7, 15, 7, 11, 15, 15}, new int[] {1, 0, 0, 1, 1, 0, 1});
	  }

	  private class AnalyzerAnonymousInnerClassHelper4 : Analyzer
	  {
		  private readonly TestWordDelimiterFilter outerInstance;

		  private int flags;

		  public AnalyzerAnonymousInnerClassHelper4(TestWordDelimiterFilter outerInstance, int flags)
		  {
			  this.outerInstance = outerInstance;
			  this.flags = flags;
		  }

		  public override TokenStreamComponents createComponents(string field, Reader reader)
		  {
			Tokenizer tokenizer = new MockTokenizer(reader, MockTokenizer.WHITESPACE, false);
			return new TokenStreamComponents(tokenizer, new WordDelimiterFilter(TEST_VERSION_CURRENT, tokenizer, flags, null));
		  }
	  }

	  /// <summary>
	  /// concat numbers + words + all + preserve original </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testLotsOfConcatenating2() throws Exception
	  public virtual void testLotsOfConcatenating2()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int flags = PRESERVE_ORIGINAL | GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | CATENATE_WORDS | CATENATE_NUMBERS | CATENATE_ALL | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;
		int flags = PRESERVE_ORIGINAL | GENERATE_WORD_PARTS | GENERATE_NUMBER_PARTS | CATENATE_WORDS | CATENATE_NUMBERS | CATENATE_ALL | SPLIT_ON_CASE_CHANGE | SPLIT_ON_NUMERICS | STEM_ENGLISH_POSSESSIVE;

		/* analyzer that uses whitespace + wdf */
		Analyzer a = new AnalyzerAnonymousInnerClassHelper5(this, flags);

		assertAnalyzesTo(a, "abc-def-123-456", new string[] {"abc-def-123-456", "abc", "abcdef", "abcdef123456", "def", "123", "123456", "456"}, new int[] {0, 0, 0, 0, 4, 8, 8, 12}, new int[] {15, 3, 7, 15, 7, 11, 15, 15}, new int[] {1, 0, 0, 0, 1, 1, 0, 1});
	  }

	  private class AnalyzerAnonymousInnerClassHelper5 : Analyzer
	  {
		  private readonly TestWordDelimiterFilter outerInstance;

		  private int flags;

		  public AnalyzerAnonymousInnerClassHelper5(TestWordDelimiterFilter outerInstance, int flags)
		  {
			  this.outerInstance = outerInstance;
			  this.flags = flags;
		  }

		  public override TokenStreamComponents createComponents(string field, Reader reader)
		  {
			Tokenizer tokenizer = new MockTokenizer(reader, MockTokenizer.WHITESPACE, false);
			return new TokenStreamComponents(tokenizer, new WordDelimiterFilter(TEST_VERSION_CURRENT, tokenizer, flags, null));
		  }
	  }

	  /// <summary>
	  /// blast some random strings through the analyzer </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testRandomStrings() throws Exception
	  public virtual void testRandomStrings()
	  {
		int numIterations = atLeast(5);
		for (int i = 0; i < numIterations; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int flags = random().nextInt(512);
		  int flags = random().Next(512);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.lucene.analysis.util.CharArraySet protectedWords;
		  CharArraySet protectedWords;
		  if (random().nextBoolean())
		  {
			protectedWords = new CharArraySet(TEST_VERSION_CURRENT, new HashSet<>("a", "b", "cd"), false);
		  }
		  else
		  {
			protectedWords = null;
		  }

		  Analyzer a = new AnalyzerAnonymousInnerClassHelper6(this, flags, protectedWords);
		  checkRandomData(random(), a, 1000 * RANDOM_MULTIPLIER);
		}
	  }

	  private class AnalyzerAnonymousInnerClassHelper6 : Analyzer
	  {
		  private readonly TestWordDelimiterFilter outerInstance;

		  private int flags;
		  private CharArraySet protectedWords;

		  public AnalyzerAnonymousInnerClassHelper6(TestWordDelimiterFilter outerInstance, int flags, CharArraySet protectedWords)
		  {
			  this.outerInstance = outerInstance;
			  this.flags = flags;
			  this.protectedWords = protectedWords;
		  }


		  protected internal override TokenStreamComponents createComponents(string fieldName, Reader reader)
		  {
			Tokenizer tokenizer = new MockTokenizer(reader, MockTokenizer.WHITESPACE, false);
			return new TokenStreamComponents(tokenizer, new WordDelimiterFilter(TEST_VERSION_CURRENT, tokenizer, flags, protectedWords));
		  }
	  }

	  /// <summary>
	  /// blast some enormous random strings through the analyzer </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testRandomHugeStrings() throws Exception
	  public virtual void testRandomHugeStrings()
	  {
		int numIterations = atLeast(5);
		for (int i = 0; i < numIterations; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int flags = random().nextInt(512);
		  int flags = random().Next(512);
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.lucene.analysis.util.CharArraySet protectedWords;
		  CharArraySet protectedWords;
		  if (random().nextBoolean())
		  {
			protectedWords = new CharArraySet(TEST_VERSION_CURRENT, new HashSet<>("a", "b", "cd"), false);
		  }
		  else
		  {
			protectedWords = null;
		  }

		  Analyzer a = new AnalyzerAnonymousInnerClassHelper7(this, flags, protectedWords);
		  checkRandomData(random(), a, 100 * RANDOM_MULTIPLIER, 8192);
		}
	  }

	  private class AnalyzerAnonymousInnerClassHelper7 : Analyzer
	  {
		  private readonly TestWordDelimiterFilter outerInstance;

		  private int flags;
		  private CharArraySet protectedWords;

		  public AnalyzerAnonymousInnerClassHelper7(TestWordDelimiterFilter outerInstance, int flags, CharArraySet protectedWords)
		  {
			  this.outerInstance = outerInstance;
			  this.flags = flags;
			  this.protectedWords = protectedWords;
		  }


		  protected internal override TokenStreamComponents createComponents(string fieldName, Reader reader)
		  {
			Tokenizer tokenizer = new MockTokenizer(reader, MockTokenizer.WHITESPACE, false);
			return new TokenStreamComponents(tokenizer, new WordDelimiterFilter(TEST_VERSION_CURRENT, tokenizer, flags, protectedWords));
		  }
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testEmptyTerm() throws java.io.IOException
	  public virtual void testEmptyTerm()
	  {
		Random random = random();
		for (int i = 0; i < 512; i++)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int flags = i;
		  int flags = i;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.apache.lucene.analysis.util.CharArraySet protectedWords;
		  CharArraySet protectedWords;
		  if (random.nextBoolean())
		  {
			protectedWords = new CharArraySet(TEST_VERSION_CURRENT, new HashSet<>("a", "b", "cd"), false);
		  }
		  else
		  {
			protectedWords = null;
		  }

		  Analyzer a = new AnalyzerAnonymousInnerClassHelper8(this, flags, protectedWords);
		  // depending upon options, this thing may or may not preserve the empty term
		  checkAnalysisConsistency(random, a, random.nextBoolean(), "");
		}
	  }

	  private class AnalyzerAnonymousInnerClassHelper8 : Analyzer
	  {
		  private readonly TestWordDelimiterFilter outerInstance;

		  private int flags;
		  private CharArraySet protectedWords;

		  public AnalyzerAnonymousInnerClassHelper8(TestWordDelimiterFilter outerInstance, int flags, CharArraySet protectedWords)
		  {
			  this.outerInstance = outerInstance;
			  this.flags = flags;
			  this.protectedWords = protectedWords;
		  }

		  protected internal override TokenStreamComponents createComponents(string fieldName, Reader reader)
		  {
			Tokenizer tokenizer = new KeywordTokenizer(reader);
			return new TokenStreamComponents(tokenizer, new WordDelimiterFilter(TEST_VERSION_CURRENT, tokenizer, flags, protectedWords));
		  }
	  }
	}

}