﻿using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace org.apache.lucene.analysis.core
{

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


	using NormalizeCharMap = org.apache.lucene.analysis.charfilter.NormalizeCharMap;
	using CJKBigramFilter = org.apache.lucene.analysis.cjk.CJKBigramFilter;
	using CommonGramsFilter = org.apache.lucene.analysis.commongrams.CommonGramsFilter;
	using CommonGramsQueryFilter = org.apache.lucene.analysis.commongrams.CommonGramsQueryFilter;
	using HyphenationCompoundWordTokenFilter = org.apache.lucene.analysis.compound.HyphenationCompoundWordTokenFilter;
	using TestCompoundWordTokenFilter = org.apache.lucene.analysis.compound.TestCompoundWordTokenFilter;
	using HyphenationTree = org.apache.lucene.analysis.compound.hyphenation.HyphenationTree;
	using Dictionary = org.apache.lucene.analysis.hunspell.Dictionary;
	using TestHunspellStemFilter = org.apache.lucene.analysis.hunspell.TestHunspellStemFilter;
	using HyphenatedWordsFilter = org.apache.lucene.analysis.miscellaneous.HyphenatedWordsFilter;
	using LimitTokenCountFilter = org.apache.lucene.analysis.miscellaneous.LimitTokenCountFilter;
	using LimitTokenPositionFilter = org.apache.lucene.analysis.miscellaneous.LimitTokenPositionFilter;
	using StemmerOverrideFilter = org.apache.lucene.analysis.miscellaneous.StemmerOverrideFilter;
	using StemmerOverrideMap = org.apache.lucene.analysis.miscellaneous.StemmerOverrideFilter.StemmerOverrideMap;
	using WordDelimiterFilter = org.apache.lucene.analysis.miscellaneous.WordDelimiterFilter;
	using EdgeNGramTokenFilter = org.apache.lucene.analysis.ngram.EdgeNGramTokenFilter;
	using Lucene43EdgeNGramTokenizer = org.apache.lucene.analysis.ngram.Lucene43EdgeNGramTokenizer;
	using PathHierarchyTokenizer = org.apache.lucene.analysis.path.PathHierarchyTokenizer;
	using ReversePathHierarchyTokenizer = org.apache.lucene.analysis.path.ReversePathHierarchyTokenizer;
	using IdentityEncoder = org.apache.lucene.analysis.payloads.IdentityEncoder;
	using PayloadEncoder = org.apache.lucene.analysis.payloads.PayloadEncoder;
	using TestSnowball = org.apache.lucene.analysis.snowball.TestSnowball;
	using StandardTokenizer = org.apache.lucene.analysis.standard.StandardTokenizer;
	using SynonymMap = org.apache.lucene.analysis.synonym.SynonymMap;
	using CharArrayMap = org.apache.lucene.analysis.util.CharArrayMap;
	using CharArraySet = org.apache.lucene.analysis.util.CharArraySet;
	using WikipediaTokenizer = org.apache.lucene.analysis.wikipedia.WikipediaTokenizer;
	using AttributeSource = org.apache.lucene.util.AttributeSource;
	using AttributeFactory = org.apache.lucene.util.AttributeSource.AttributeFactory;
	using CharsRef = org.apache.lucene.util.CharsRef;
	using Rethrow = org.apache.lucene.util.Rethrow;
	using TestUtil = org.apache.lucene.util.TestUtil;
	using Version = org.apache.lucene.util.Version;
	using CharacterRunAutomaton = org.apache.lucene.util.automaton.CharacterRunAutomaton;
	using AfterClass = org.junit.AfterClass;
	using BeforeClass = org.junit.BeforeClass;
	using SnowballProgram = org.tartarus.snowball.SnowballProgram;
	using InputSource = org.xml.sax.InputSource;

	/// <summary>
	/// tests random analysis chains </summary>
	public class TestRandomChains : BaseTokenStreamTestCase
	{

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static java.util.List<Constructor<? extends org.apache.lucene.analysis.Tokenizer>> tokenizers;
	  internal static IList<Constructor<?>> tokenizers;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static java.util.List<Constructor<? extends org.apache.lucene.analysis.TokenFilter>> tokenfilters;
	  internal static IList<Constructor<?>> tokenfilters;
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: static java.util.List<Constructor<? extends org.apache.lucene.analysis.CharFilter>> charfilters;
	  internal static IList<Constructor<?>> charfilters;

	  private interface Predicate<T>
	  {
		bool apply(T o);
	  }

	  private static readonly Predicate<object[]> ALWAYS = new PredicateAnonymousInnerClassHelper();

	  private class PredicateAnonymousInnerClassHelper : Predicate<object[]>
	  {
		  public PredicateAnonymousInnerClassHelper()
		  {
		  }

		  public virtual bool apply(object[] args)
		  {
			return true;
		  };
	  }

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static final java.util.Map<Constructor<?>,Predicate<Object[]>> brokenConstructors = new java.util.HashMap<>();
	  private static readonly IDictionary<Constructor<?>, Predicate<object[]>> brokenConstructors = new Dictionary<Constructor<?>, Predicate<object[]>>();
	  static TestRandomChains()
	  {
		try
		{
		  brokenConstructors[typeof(LimitTokenCountFilter).GetConstructor(typeof(TokenStream), typeof(int))] = ALWAYS;
		  brokenConstructors[typeof(LimitTokenCountFilter).GetConstructor(typeof(TokenStream), typeof(int), typeof(bool))] = new PredicateAnonymousInnerClassHelper2();
		  brokenConstructors[typeof(LimitTokenPositionFilter).GetConstructor(typeof(TokenStream), typeof(int))] = ALWAYS;
		  brokenConstructors[typeof(LimitTokenPositionFilter).GetConstructor(typeof(TokenStream), typeof(int), typeof(bool))] = new PredicateAnonymousInnerClassHelper3();
		  foreach (Type c in Arrays.asList<Type>(typeof(CachingTokenFilter), typeof(CrankyTokenFilter), typeof(ValidatingTokenFilter)))
			  // TODO: can we promote some of these to be only
			  // offsets offenders?
			  // doesn't actual reset itself!
			  // Not broken, simulates brokenness:
			  // Not broken: we forcefully add this, so we shouldn't
			  // also randomly pick it:
		  {
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Constructor<?> ctor : c.getConstructors())
			foreach (Constructor<?> ctor in c.GetConstructors())
			{
			  brokenConstructors[ctor] = ALWAYS;
			}
		  }
		}
		catch (Exception e)
		{
		  throw new Exception(e);
		}
		try
		{
		  foreach (Type c in Arrays.asList<Type>(typeof(ReversePathHierarchyTokenizer), typeof(PathHierarchyTokenizer), typeof(WikipediaTokenizer), typeof(CJKBigramFilter), typeof(HyphenatedWordsFilter), typeof(CommonGramsFilter), typeof(CommonGramsQueryFilter), typeof(WordDelimiterFilter)))
			  // TODO: it seems to mess up offsets!?
			  // TODO: doesn't handle graph inputs
			  // TODO: doesn't handle graph inputs (or even look at positionIncrement)
			  // TODO: LUCENE-4983
			  // TODO: doesn't handle graph inputs
			  // TODO: probably doesnt handle graph inputs, too afraid to try
		  {
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (Constructor<?> ctor : c.getConstructors())
			foreach (Constructor<?> ctor in c.GetConstructors())
			{
			  brokenOffsetsConstructors[ctor] = ALWAYS;
			}
		  }
		}
		catch (Exception e)
		{
		  throw new Exception(e);
		}
		allowedTokenizerArgs = Collections.newSetFromMap(new IdentityHashMap<Type, bool?>());
		allowedTokenizerArgs.addAll(argProducers.Keys);
		allowedTokenizerArgs.Add(typeof(Reader));
		allowedTokenizerArgs.Add(typeof(AttributeSource.AttributeFactory));
		allowedTokenizerArgs.Add(typeof(AttributeSource));

		allowedTokenFilterArgs = Collections.newSetFromMap(new IdentityHashMap<Type, bool?>());
		allowedTokenFilterArgs.addAll(argProducers.Keys);
		allowedTokenFilterArgs.Add(typeof(TokenStream));
		// TODO: fix this one, thats broken:
		allowedTokenFilterArgs.Add(typeof(CommonGramsFilter));

		allowedCharFilterArgs = Collections.newSetFromMap(new IdentityHashMap<Type, bool?>());
		allowedCharFilterArgs.addAll(argProducers.Keys);
		allowedCharFilterArgs.Add(typeof(Reader));
	  }

	  private class PredicateAnonymousInnerClassHelper2 : Predicate<object[]>
	  {
		  public PredicateAnonymousInnerClassHelper2()
		  {
		  }

		  public virtual bool apply(object[] args)
		  {
			Debug.Assert(args.Length == 3);
			return !((bool?) args[2]); // args are broken if consumeAllTokens is false
		  }
	  }

	  private class PredicateAnonymousInnerClassHelper3 : Predicate<object[]>
	  {
		  public PredicateAnonymousInnerClassHelper3()
		  {
		  }

		  public virtual bool apply(object[] args)
		  {
			Debug.Assert(args.Length == 3);
			return !((bool?) args[2]); // args are broken if consumeAllTokens is false
		  }
	  }

	  // TODO: also fix these and remove (maybe):
	  // Classes/options that don't produce consistent graph offsets:
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: private static final java.util.Map<Constructor<?>,Predicate<Object[]>> brokenOffsetsConstructors = new java.util.HashMap<>();
	  private static readonly IDictionary<Constructor<?>, Predicate<object[]>> brokenOffsetsConstructors = new Dictionary<Constructor<?>, Predicate<object[]>>();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @BeforeClass public static void beforeClass() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public static void beforeClass()
	  {
		IList<Type> analysisClasses = getClassesForPackage("org.apache.lucene.analysis");
		tokenizers = new List<>();
		tokenfilters = new List<>();
		charfilters = new List<>();
		foreach (Class c in analysisClasses)
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final int modifiers = c.getModifiers();
		  int modifiers = c.Modifiers;
		  if (Modifier.isAbstract(modifiers) || !Modifier.isPublic(modifiers) || c.Synthetic || c.AnonymousClass || c.MemberClass || c.Interface || c.isAnnotationPresent(typeof(Deprecated)) || !(c.IsSubclassOf(typeof(Tokenizer)) || c.IsSubclassOf(typeof(TokenFilter)) || c.IsSubclassOf(typeof(CharFilter))))
		  {
			// don't waste time with abstract classes or deprecated known-buggy ones
			continue;
		  }

//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
//ORIGINAL LINE: for (final Constructor<?> ctor : c.getConstructors())
		  foreach (Constructor<?> ctor in c.Constructors)
		  {
			// don't test synthetic or deprecated ctors, they likely have known bugs:
			if (ctor.Synthetic || ctor.isAnnotationPresent(typeof(Deprecated)) || brokenConstructors[ctor] == ALWAYS)
			{
			  continue;
			}
			if (c.IsSubclassOf(typeof(Tokenizer)))
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
			  assertTrue(ctor.toGenericString() + " has unsupported parameter types", allowedTokenizerArgs.containsAll(Arrays.asList(ctor.ParameterTypes)));
			  tokenizers.Add(castConstructor(typeof(Tokenizer), ctor));
			}
			else if (c.IsSubclassOf(typeof(TokenFilter)))
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
			  assertTrue(ctor.toGenericString() + " has unsupported parameter types", allowedTokenFilterArgs.containsAll(Arrays.asList(ctor.ParameterTypes)));
			  tokenfilters.Add(castConstructor(typeof(TokenFilter), ctor));
			}
			else if (c.IsSubclassOf(typeof(CharFilter)))
			{
//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
			  assertTrue(ctor.toGenericString() + " has unsupported parameter types", allowedCharFilterArgs.containsAll(Arrays.asList(ctor.ParameterTypes)));
			  charfilters.Add(castConstructor(typeof(CharFilter), ctor));
			}
			else
			{
			  fail("Cannot get here");
			}
		  }
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Comparator<Constructor<?>> ctorComp = new java.util.Comparator<Constructor<?>>()
//JAVA TO C# CONVERTER TODO TASK: Java wildcard generics are not converted to .NET:
		IComparer<Constructor<?>> ctorComp = new ComparatorAnonymousInnerClassHelper();
		tokenizers.Sort(ctorComp);
		tokenfilters.Sort(ctorComp);
		charfilters.Sort(ctorComp);
		if (VERBOSE)
		{
		  Console.WriteLine("tokenizers = " + tokenizers);
		  Console.WriteLine("tokenfilters = " + tokenfilters);
		  Console.WriteLine("charfilters = " + charfilters);
		}
	  }

	  private class ComparatorAnonymousInnerClassHelper : IComparer<Constructor<JavaToDotNetGenericWildcard>>
	  {
		  public ComparatorAnonymousInnerClassHelper()
		  {
		  }

		  public virtual int compare<T1, T2>(Constructor<T1> arg0, Constructor<T2> arg1)
		  {
			return arg0.toGenericString().compareTo(arg1.toGenericString());
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void afterClass()
	  public static void afterClass()
	  {
		tokenizers = null;
		tokenfilters = null;
		charfilters = null;
	  }

	  /// <summary>
	  /// Hack to work around the stupidness of Oracle's strict Java backwards compatibility.
	  /// {@code Class<T>#getConstructors()} should return unmodifiable {@code List<Constructor<T>>} not array! 
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") private static <T> Constructor<T> castConstructor(Class<T> instanceClazz, Constructor<?> ctor)
	  private static Constructor<T> castConstructor<T, T1>(Type<T> instanceClazz, Constructor<T1> ctor)
	  {
		return (Constructor<T>) ctor;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public static java.util.List<Class> getClassesForPackage(String pckgname) throws Exception
	  public static IList<Type> getClassesForPackage(string pckgname)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<Class> classes = new java.util.ArrayList<>();
		IList<Type> classes = new List<Type>();
		collectClassesForPackage(pckgname, classes);
		assertFalse("No classes found in package '" + pckgname + "'; maybe your test classes are packaged as JAR file?", classes.Count == 0);
		return classes;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private static void collectClassesForPackage(String pckgname, java.util.List<Class> classes) throws Exception
	  private static void collectClassesForPackage(string pckgname, IList<Type> classes)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final ClassLoader cld = TestRandomChains.class.getClassLoader();
		ClassLoader cld = typeof(TestRandomChains).ClassLoader;
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String path = pckgname.replace('.', '/');
		string path = pckgname.Replace('.', '/');
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Iterator<java.net.URL> resources = cld.getResources(path);
		IEnumerator<URL> resources = cld.getResources(path);
		while (resources.MoveNext())
		{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.net.URI uri = resources.Current.toURI();
		  URI uri = resources.Current.toURI();
		  if (!"file".Equals(uri.Scheme, StringComparison.CurrentCultureIgnoreCase))
		  {
			continue;
		  }
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.io.File directory = new java.io.File(uri);
		  File directory = new File(uri);
		  if (directory.exists())
		  {
			string[] files = directory.list();
			foreach (string file in files)
			{
			  if ((new File(directory, file)).Directory)
			  {
				// recurse
				string subPackage = pckgname + "." + file;
				collectClassesForPackage(subPackage, classes);
			  }
			  if (file.EndsWith(".class", StringComparison.Ordinal))
			  {
				string clazzName = file.Substring(0, file.Length - 6);
				// exclude Test classes that happen to be in these packages.
				// class.ForName'ing some of them can cause trouble.
				if (!clazzName.EndsWith("Test", StringComparison.Ordinal) && !clazzName.StartsWith("Test", StringComparison.Ordinal))
				{
				  // Don't run static initializers, as we won't use most of them.
				  // Java will do that automatically once accessed/instantiated.
				  classes.Add(Type.GetType(pckgname + '.' + clazzName, false, cld));
				}
			  }
			}
		  }
		}
	  }

	  private interface ArgProducer
	  {
		object create(Random random);
	  }

	  private static readonly IDictionary<Type, ArgProducer> argProducers = new IdentityHashMapAnonymousInnerClassHelper();

	  private class IdentityHashMapAnonymousInnerClassHelper : IdentityHashMap<Type, ArgProducer>
	  {
		  public IdentityHashMapAnonymousInnerClassHelper()
		  {
		  }

	//	  {
	//	put(int.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		// TODO: could cause huge ram usage to use full int range for some filters
	//		// (e.g. allocate enormous arrays)
	//		// return Integer.valueOf(random.nextInt());
	//		return Integer.valueOf(TestUtil.nextInt(random, -100, 100));
	//	  }
	//	}
	//   );
	//	put(char.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		// TODO: fix any filters that care to throw IAE instead.
	//		// also add a unicode validating filter to validate termAtt?
	//		// return Character.valueOf((char)random.nextInt(65536));
	//		while(true)
	//		{
	//		  char c = (char)random.nextInt(65536);
	//		  if (c < '\uD800' || c > '\uDFFF')
	//		  {
	//			return Character.valueOf(c);
	//		  }
	//		}
	//	  }
	//	}
	//   );
	//	put(float.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		return Float.valueOf(random.nextFloat());
	//	  }
	//	}
	//   );
	//	put(boolean.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		return Boolean.valueOf(random.nextBoolean());
	//	  }
	//	}
	//   );
	//	put(byte.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		// this wraps to negative when casting to byte
	//		return Byte.valueOf((byte) random.nextInt(256));
	//	  }
	//	}
	//   );
	//	put(byte[].class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		byte bytes[] = new byte[random.nextInt(256)];
	//		random.nextBytes(bytes);
	//		return bytes;
	//	  }
	//	}
	//   );
	//	put(Random.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		return new Random(random.nextLong());
	//	  }
	//	}
	//   );
	//	put(Version.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		// we expect bugs in emulating old versions
	//		return TEST_VERSION_CURRENT;
	//	  }
	//	}
	//   );
	//	put(Set.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		// TypeTokenFilter
	//		Set<String> set = new HashSet<>();
	//		int num = random.nextInt(5);
	//		for (int i = 0; i < num; i++)
	//		{
	//		  set.add(StandardTokenizer.TOKEN_TYPES[random.nextInt(StandardTokenizer.TOKEN_TYPES.length)]);
	//		}
	//		return set;
	//	  }
	//	}
	//   );
	//	put(Collection.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		// CapitalizationFilter
	//		Collection<char[]> col = new ArrayList<>();
	//		int num = random.nextInt(5);
	//		for (int i = 0; i < num; i++)
	//		{
	//		  col.add(TestUtil.randomSimpleString(random).toCharArray());
	//		}
	//		return col;
	//	  }
	//	}
	//   );
	//	put(CharArraySet.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		int num = random.nextInt(10);
	//		CharArraySet set = new CharArraySet(TEST_VERSION_CURRENT, num, random.nextBoolean());
	//		for (int i = 0; i < num; i++)
	//		{
	//		  // TODO: make nastier
	//		  set.add(TestUtil.randomSimpleString(random));
	//		}
	//		return set;
	//	  }
	//	}
	//   );
	//	put(Pattern.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		// TODO: don't want to make the exponentially slow ones Dawid documents
	//		// in TestPatternReplaceFilter, so dont use truly random patterns (for now)
	//		return Pattern.compile("a");
	//	  }
	//	}
	//   );
	//
	//	put(Pattern[].class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		return new Pattern[] {Pattern.compile("([a-z]+)"), Pattern.compile("([0-9]+)")};
	//	  }
	//	}
	//   );
	//	put(PayloadEncoder.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		return new IdentityEncoder(); // the other encoders will throw exceptions if tokens arent numbers?
	//	  }
	//	}
	//   );
	//	put(Dictionary.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		// TODO: make nastier
	//		InputStream affixStream = TestHunspellStemFilter.class.getResourceAsStream("simple.aff");
	//		InputStream dictStream = TestHunspellStemFilter.class.getResourceAsStream("simple.dic");
	//		try
	//		{
	//		 return new Dictionary(affixStream, dictStream);
	//		}
	//		catch (Exception ex)
	//		{
	//		  Rethrow.rethrow(ex);
	//		  return null; // unreachable code
	//		}
	//	  }
	//	}
	//   );
	//	put(Lucene43EdgeNGramTokenizer.Side.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		return random.nextBoolean() ? Lucene43EdgeNGramTokenizer.Side.FRONT : Lucene43EdgeNGramTokenizer.Side.BACK;
	//	  }
	//	}
	//   );
	//	put(EdgeNGramTokenFilter.Side.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		return random.nextBoolean() ? EdgeNGramTokenFilter.Side.FRONT : EdgeNGramTokenFilter.Side.BACK;
	//	  }
	//	}
	//   );
	//	put(HyphenationTree.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		// TODO: make nastier
	//		try
	//		{
	//		  InputSource @is = new InputSource(TestCompoundWordTokenFilter.class.getResource("da_UTF8.xml").toExternalForm());
	//		  HyphenationTree hyphenator = HyphenationCompoundWordTokenFilter.getHyphenationTree(@is);
	//		  return hyphenator;
	//		}
	//		catch (Exception ex)
	//		{
	//		  Rethrow.rethrow(ex);
	//		  return null; // unreachable code
	//		}
	//	  }
	//	}
	//   );
	//	put(SnowballProgram.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		try
	//		{
	//		  String lang = TestSnowball.SNOWBALL_LANGS[random.nextInt(TestSnowball.SNOWBALL_LANGS.length)];
	//		  Class<? extends SnowballProgram> clazz = Class.forName("org.tartarus.snowball.ext." + lang + "Stemmer").asSubclass(SnowballProgram.class);
	//		  return clazz.newInstance();
	//		}
	//		catch (Exception ex)
	//		{
	//		  Rethrow.rethrow(ex);
	//		  return null; // unreachable code
	//		}
	//	  }
	//	}
	//   );
	//	put(String.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		// TODO: make nastier
	//		if (random.nextBoolean())
	//		{
	//		  // a token type
	//		  return StandardTokenizer.TOKEN_TYPES[random.nextInt(StandardTokenizer.TOKEN_TYPES.length)];
	//		}
	//		else
	//		{
	//		  return TestUtil.randomSimpleString(random);
	//		}
	//	  }
	//	}
	//   );
	//	put(NormalizeCharMap.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		NormalizeCharMap.Builder builder = new NormalizeCharMap.Builder();
	//		// we can't add duplicate keys, or NormalizeCharMap gets angry
	//		Set<String> keys = new HashSet<>();
	//		int num = random.nextInt(5);
	//		//System.out.println("NormalizeCharMap=");
	//		for (int i = 0; i < num; i++)
	//		{
	//		  String key = TestUtil.randomSimpleString(random);
	//		  if (!keys.contains(key) && key.length() > 0)
	//		  {
	//			String value = TestUtil.randomSimpleString(random);
	//			builder.add(key, value);
	//			keys.add(key);
	//			//System.out.println("mapping: '" + key + "' => '" + value + "'");
	//		  }
	//		}
	//		return builder.build();
	//	  }
	//	}
	//   );
	//	put(CharacterRunAutomaton.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		// TODO: could probably use a purely random automaton
	//		switch(random.nextInt(5))
	//		{
	//		  case 0:
	//			  return MockTokenizer.KEYWORD;
	//		  case 1:
	//			  return MockTokenizer.SIMPLE;
	//		  case 2:
	//			  return MockTokenizer.WHITESPACE;
	//		  case 3:
	//			  return MockTokenFilter.EMPTY_STOPSET;
	//		  default:
	//			  return MockTokenFilter.ENGLISH_STOPSET;
	//		}
	//	  }
	//	}
	//   );
	//	put(CharArrayMap.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		int num = random.nextInt(10);
	//		CharArrayMap<String> map = new CharArrayMap<>(TEST_VERSION_CURRENT, num, random.nextBoolean());
	//		for (int i = 0; i < num; i++)
	//		{
	//		  // TODO: make nastier
	//		  map.put(TestUtil.randomSimpleString(random), TestUtil.randomSimpleString(random));
	//		}
	//		return map;
	//	  }
	//	}
	//   );
	//	put(StemmerOverrideMap.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		int num = random.nextInt(10);
	//		StemmerOverrideFilter.Builder builder = new StemmerOverrideFilter.Builder(random.nextBoolean());
	//		for (int i = 0; i < num; i++)
	//		{
	//		  String input = "";
	//		  do
	//		  {
	//			input = TestUtil.randomRealisticUnicodeString(random);
	//		  } while(input.isEmpty());
	//		  String @out = "";
	//		  TestUtil.randomSimpleString(random);
	//		  do
	//		  {
	//			@out = TestUtil.randomRealisticUnicodeString(random);
	//		  } while(@out.isEmpty());
	//		  builder.add(input, @out);
	//		}
	//		try
	//		{
	//		  return builder.build();
	//		}
	//		catch (Exception ex)
	//		{
	//		  Rethrow.rethrow(ex);
	//		  return null; // unreachable code
	//		}
	//	  }
	//	}
	//   );
	//	put(SynonymMap.class, new ArgProducer()
	//	{
	//	  @@Override public Object create(Random random)
	//	  {
	//		SynonymMap.Builder b = new SynonymMap.Builder(random.nextBoolean());
	//		final int numEntries = atLeast(10);
	//		for (int j = 0; j < numEntries; j++)
	//		{
	//		  addSyn(b, randomNonEmptyString(random), randomNonEmptyString(random), random.nextBoolean());
	//		}
	//		try
	//		{
	//		  return b.build();
	//		}
	//		catch (Exception ex)
	//		{
	//		  Rethrow.rethrow(ex);
	//		  return null; // unreachable code
	//		}
	//	  }
	//
	//	  private void addSyn(SynonymMap.Builder b, String input, String output, boolean keepOrig)
	//	  {
	//		b.add(new CharsRef(input.replaceAll(" +", "\u0000")), new CharsRef(output.replaceAll(" +", "\u0000")), keepOrig);
	//	  }
	//
	//	  private String randomNonEmptyString(Random random)
	//	  {
	//		while(true)
	//		{
	//		  final String s = TestUtil.randomUnicodeString(random).trim();
	//		  if (s.length() != 0 && s.indexOf('\u0000') == -1)
	//		  {
	//			return s;
	//		  }
	//		}
	//	  }
	//	}
	//   );
	//  }
	//  }
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//
	//  static final Set<Class> allowedTokenizerArgs, allowedTokenFilterArgs, allowedCharFilterArgs;
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//ignore
	//
	//  @@SuppressWarnings("unchecked") static <T> T newRandomArg(Random random, Class<T> paramType)
	//  {
	//	final ArgProducer producer = argProducers.get(paramType);
	//	assertNotNull("No producer for arguments of type " + paramType.getName() + " found", producer);
	//	return (T) producer.create(random);
	//  }
	//
	//  static Object[] newTokenizerArgs(Random random, Reader reader, Class[] paramTypes)
	//  {
	//	Object[] args = new Object[paramTypes.length];
	//	for (int i = 0; i < args.length; i++)
	//	{
	//	  Class paramType = paramTypes[i];
	//	  if (paramType == Reader.class)
	//	  {
	//		args[i] = reader;
	//	  }
	//	  else if (paramType == AttributeFactory.class)
	//	  {
	//		// TODO: maybe the collator one...???
	//		args[i] = AttributeFactory.DEFAULT_ATTRIBUTE_FACTORY;
	//	  }
	//	  else if (paramType == AttributeSource.class)
	//	  {
	//		// TODO: args[i] = new AttributeSource();
	//		// this is currently too scary to deal with!
	//		args[i] = null; // force IAE
	//	  }
	//	  else
	//	  {
	//		args[i] = newRandomArg(random, paramType);
	//	  }
	//	}
	//	return args;
	//  }
	//
	//  static Object[] newCharFilterArgs(Random random, Reader reader, Class[] paramTypes)
	//  {
	//	Object[] args = new Object[paramTypes.length];
	//	for (int i = 0; i < args.length; i++)
	//	{
	//	  Class paramType = paramTypes[i];
	//	  if (paramType == Reader.class)
	//	  {
	//		args[i] = reader;
	//	  }
	//	  else
	//	  {
	//		args[i] = newRandomArg(random, paramType);
	//	  }
	//	}
	//	return args;
	//  }
	//
	//  static Object[] newFilterArgs(Random random, TokenStream stream, Class[] paramTypes)
	//  {
	//	Object[] args = new Object[paramTypes.length];
	//	for (int i = 0; i < args.length; i++)
	//	{
	//	  Class paramType = paramTypes[i];
	//	  if (paramType == TokenStream.class)
	//	  {
	//		args[i] = stream;
	//	  }
	//	  else if (paramType == CommonGramsFilter.class)
	//	  {
	//		// TODO: fix this one, thats broken: CommonGramsQueryFilter takes this one explicitly
	//		args[i] = new CommonGramsFilter(TEST_VERSION_CURRENT, stream, newRandomArg(random, CharArraySet.class));
	//	  }
	//	  else
	//	  {
	//		args[i] = newRandomArg(random, paramType);
	//	  }
	//	}
	//	return args;
	//  }
	//
	//  static class MockRandomAnalyzer extends Analyzer
	//  {
	//	final long seed;
	//
	//	MockRandomAnalyzer(long seed)
	//	{
	//	  this.seed = seed;
	//	}
	//
	//	public boolean offsetsAreCorrect()
	//	{
	//	  // TODO: can we not do the full chain here!?
	//	  Random random = new Random(seed);
	//	  TokenizerSpec tokenizerSpec = newTokenizer(random, new StringReader(""));
	//	  TokenFilterSpec filterSpec = newFilterChain(random, tokenizerSpec.tokenizer, tokenizerSpec.offsetsAreCorrect);
	//	  return filterSpec.offsetsAreCorrect;
	//	}
	//
	//	@@Override protected TokenStreamComponents createComponents(String fieldName, Reader reader)
	//	{
	//	  Random random = new Random(seed);
	//	  TokenizerSpec tokenizerSpec = newTokenizer(random, reader);
	//	  //System.out.println("seed=" + seed + ",create tokenizer=" + tokenizerSpec.toString);
	//	  TokenFilterSpec filterSpec = newFilterChain(random, tokenizerSpec.tokenizer, tokenizerSpec.offsetsAreCorrect);
	//	  //System.out.println("seed=" + seed + ",create filter=" + filterSpec.toString);
	//	  return new TokenStreamComponents(tokenizerSpec.tokenizer, filterSpec.stream);
	//	}
	//
	//	@@Override protected Reader initReader(String fieldName, Reader reader)
	//	{
	//	  Random random = new Random(seed);
	//	  CharFilterSpec charfilterspec = newCharFilterChain(random, reader);
	//	  return charfilterspec.reader;
	//	}
	//
	//	@@Override public String toString()
	//	{
	//	  Random random = new Random(seed);
	//	  StringBuilder sb = new StringBuilder();
	//	  CharFilterSpec charFilterSpec = newCharFilterChain(random, new StringReader(""));
	//	  sb.append("\ncharfilters=");
	//	  sb.append(charFilterSpec.toString);
	//	  // intentional: initReader gets its own separate random
	//	  random = new Random(seed);
	//	  TokenizerSpec tokenizerSpec = newTokenizer(random, charFilterSpec.reader);
	//	  sb.append("\n");
	//	  sb.append("tokenizer=");
	//	  sb.append(tokenizerSpec.toString);
	//	  TokenFilterSpec tokenFilterSpec = newFilterChain(random, tokenizerSpec.tokenizer, tokenizerSpec.offsetsAreCorrect);
	//	  sb.append("\n");
	//	  sb.append("filters=");
	//	  sb.append(tokenFilterSpec.toString);
	//	  sb.append("\n");
	//	  sb.append("offsetsAreCorrect=" + tokenFilterSpec.offsetsAreCorrect);
	//	  return sb.toString();
	//	}
	//
	//	private <T> T createComponent(Constructor<T> ctor, Object[] args, StringBuilder descr)
	//	{
	//	  try
	//	  {
	//		final T instance = ctor.newInstance(args);
	// /*
	// if (descr.length() > 0) {
	//   descr.append(",");
	// }
	// */
	//		descr.append("\n  ");
	//		descr.append(ctor.getDeclaringClass().getName());
	//		String @params = Arrays.deepToString(args);
	//		@params = @params.substring(1, (@params.length()-1) - 1);
	//		descr.append("(").append(@params).append(")");
	//		return instance;
	//	  }
	//	  catch (InvocationTargetException ite)
	//	  {
	//		final Throwable cause = ite.getCause();
	//		if (cause instanceof IllegalArgumentException || cause instanceof UnsupportedOperationException)
	//	{
	//		  // thats ok, ignore
	//		  if (VERBOSE)
	//		  {
	//			System.err.println("Ignoring IAE/UOE from ctor:");
	//			cause.printStackTrace(System.err);
	//		  }
	//		}
	//		else
	//		{
	//		  Rethrow.rethrow(cause);
	//		}
	//	  }
	//	  catch (IllegalAccessException iae)
	//	  {
	//		Rethrow.rethrow(iae);
	//	  }
	//	  catch (InstantiationException ie)
	//	  {
	//		Rethrow.rethrow(ie);
	//	  }
	//	  return null; // no success
	//	}
	//
	//	private boolean broken(Constructor<?> ctor, Object[] args)
	//	{
	//	  final Predicate<Object[]> pred = brokenConstructors.get(ctor);
	//	  return pred != null && pred.apply(args);
	//	}
	//
	//	private boolean brokenOffsets(Constructor<?> ctor, Object[] args)
	//	{
	//	  final Predicate<Object[]> pred = brokenOffsetsConstructors.get(ctor);
	//	  return pred != null && pred.apply(args);
	//	}
	//
	//	// create a new random tokenizer from classpath
	//	private TokenizerSpec newTokenizer(Random random, Reader reader)
	//	{
	//	  TokenizerSpec spec = new TokenizerSpec();
	//	  while (spec.tokenizer == null)
	//	  {
	//		final Constructor<? extends Tokenizer> ctor = tokenizers.get(random.nextInt(tokenizers.size()));
	//		final StringBuilder descr = new StringBuilder();
	//		final CheckThatYouDidntReadAnythingReaderWrapper wrapper = new CheckThatYouDidntReadAnythingReaderWrapper(reader);
	//		final Object args[] = newTokenizerArgs(random, wrapper, ctor.getParameterTypes());
	//		if (broken(ctor, args))
	//		{
	//		  continue;
	//		}
	//		spec.tokenizer = createComponent(ctor, args, descr);
	//		if (spec.tokenizer != null)
	//		{
	//		  spec.offsetsAreCorrect &= !brokenOffsets(ctor, args);
	//		  spec.toString = descr.toString();
	//		}
	//		else
	//		{
	//		  assertFalse(ctor.getDeclaringClass().getName() + " has read something in ctor but failed with UOE/IAE", wrapper.readSomething);
	//		}
	//	  }
	//	  return spec;
	//	}
	//
	//	private CharFilterSpec newCharFilterChain(Random random, Reader reader)
	//	{
	//	  CharFilterSpec spec = new CharFilterSpec();
	//	  spec.reader = reader;
	//	  StringBuilder descr = new StringBuilder();
	//	  int numFilters = random.nextInt(3);
	//	  for (int i = 0; i < numFilters; i++)
	//	  {
	//		while (true)
	//		{
	//		  final Constructor<? extends CharFilter> ctor = charfilters.get(random.nextInt(charfilters.size()));
	//		  final Object args[] = newCharFilterArgs(random, spec.reader, ctor.getParameterTypes());
	//		  if (broken(ctor, args))
	//		  {
	//			continue;
	//		  }
	//		  reader = createComponent(ctor, args, descr);
	//		  if (reader != null)
	//		  {
	//			spec.reader = reader;
	//			break;
	//		  }
	//		}
	//	  }
	//	  spec.toString = descr.toString();
	//	  return spec;
	//	}
	//
	//	private TokenFilterSpec newFilterChain(Random random, Tokenizer tokenizer, boolean offsetsAreCorrect)
	//	{
	//	  TokenFilterSpec spec = new TokenFilterSpec();
	//	  spec.offsetsAreCorrect = offsetsAreCorrect;
	//	  spec.stream = tokenizer;
	//	  StringBuilder descr = new StringBuilder();
	//	  int numFilters = random.nextInt(5);
	//	  for (int i = 0; i < numFilters; i++)
	//	  {
	//
	//		// Insert ValidatingTF after each stage so we can
	//		// catch problems right after the TF that "caused"
	//		// them:
	//		spec.stream = new ValidatingTokenFilter(spec.stream, "stage " + i, spec.offsetsAreCorrect);
	//
	//		while (true)
	//		{
	//		  final Constructor<? extends TokenFilter> ctor = tokenfilters.get(random.nextInt(tokenfilters.size()));
	//
	//		  // hack: MockGraph/MockLookahead has assertions that will trip if they follow
	//		  // an offsets violator. so we cant use them after e.g. wikipediatokenizer
	//		  if (!spec.offsetsAreCorrect && (ctor.getDeclaringClass().equals(MockGraphTokenFilter.class) || ctor.getDeclaringClass().equals(MockRandomLookaheadTokenFilter.class)))
	//		  {
	//			continue;
	//		  }
	//
	//		  final Object args[] = newFilterArgs(random, spec.stream, ctor.getParameterTypes());
	//		  if (broken(ctor, args))
	//		  {
	//			continue;
	//		  }
	//		  final TokenFilter flt = createComponent(ctor, args, descr);
	//		  if (flt != null)
	//		  {
	//			spec.offsetsAreCorrect &= !brokenOffsets(ctor, args);
	//			spec.stream = flt;
	//			break;
	//		  }
	//		}
	//	  }
	//
	//	  // Insert ValidatingTF after each stage so we can
	//	  // catch problems right after the TF that "caused"
	//	  // them:
	//	  spec.stream = new ValidatingTokenFilter(spec.stream, "last stage", spec.offsetsAreCorrect);
	//
	//	  spec.toString = descr.toString();
	//	  return spec;
	//	}
	//  }
	//
	//  static class CheckThatYouDidntReadAnythingReaderWrapper extends CharFilter
	//  {
	//	boolean readSomething;
	//
	//	CheckThatYouDidntReadAnythingReaderWrapper(Reader @in)
	//	{
	//	  base(@in);
	//	}
	//
	//	@@Override public int correct(int currentOff)
	//	{
	//	  return currentOff; // we don't change any offsets
	//	}
	//
	//	@@Override public int read(char[] cbuf, int off, int len) throws IOException
	//	{
	//	  readSomething = true;
	//	  return input.read(cbuf, off, len);
	//	}
	//
	//	@@Override public int read() throws IOException
	//	{
	//	  readSomething = true;
	//	  return input.read();
	//	}
	//
	//	@@Override public int read(CharBuffer target) throws IOException
	//	{
	//	  readSomething = true;
	//	  return input.read(target);
	//	}
	//
	//	@@Override public int read(char[] cbuf) throws IOException
	//	{
	//	  readSomething = true;
	//	  return input.read(cbuf);
	//	}
	//
	//	@@Override public long skip(long n) throws IOException
	//	{
	//	  readSomething = true;
	//	  return input.skip(n);
	//	}
	//
	//	@@Override public void mark(int readAheadLimit) throws IOException
	//	{
	//	  input.mark(readAheadLimit);
	//	}
	//
	//	@@Override public boolean markSupported()
	//	{
	//	  return input.markSupported();
	//	}
	//
	//	@@Override public boolean ready() throws IOException
	//	{
	//	  return input.ready();
	//	}
	//
	//	@@Override public void reset() throws IOException
	//	{
	//	  input.reset();
	//	}
	//  }
	//
	//  static class TokenizerSpec
	//  {
	//	Tokenizer tokenizer;
	//	String toString;
	//	boolean offsetsAreCorrect = true;
	//  }
	//
	//  static class TokenFilterSpec
	//  {
	//	TokenStream stream;
	//	String toString;
	//	boolean offsetsAreCorrect = true;
	//  }
	//
	//  static class CharFilterSpec
	//  {
	//	Reader reader;
	//	String toString;
	//  }
	//
	//  public void testRandomChains() throws Throwable
	//  {
	//	int numIterations = atLeast(20);
	//	Random random = random();
	//	for (int i = 0; i < numIterations; i++)
	//	{
	//	  MockRandomAnalyzer a = new MockRandomAnalyzer(random.nextLong());
	//	  if (VERBOSE)
	//	  {
	//		System.out.println("Creating random analyzer:" + a);
	//	  }
	//	  try
	//	  {
	//		checkRandomData(random, a, 500*RANDOM_MULTIPLIER, 20, false, false); // We already validate our own offsets...
	//	  }
	//	  catch (Throwable e)
	//	  {
	//		System.err.println("Exception from random analyzer: " + a);
	//		throw e;
	//	  }
	//	}
	//  }
	//
	//  // we might regret this decision...
	//  public void testRandomChainsWithLargeStrings() throws Throwable
	//  {
	//	int numIterations = atLeast(20);
	//	Random random = random();
	//	for (int i = 0; i < numIterations; i++)
	//	{
	//	  MockRandomAnalyzer a = new MockRandomAnalyzer(random.nextLong());
	//	  if (VERBOSE)
	//	  {
	//		System.out.println("Creating random analyzer:" + a);
	//	  }
	//	  try
	//	  {
	//		checkRandomData(random, a, 50*RANDOM_MULTIPLIER, 128, false, false); // We already validate our own offsets...
	//	  }
	//	  catch (Throwable e)
	//	  {
	//		System.err.println("Exception from random analyzer: " + a);
	//		throw e;
	//	  }
	//	}
	//  }
	//}

	  }