#if DEBUGTEST
using System;
using System.Globalization;
using QPP.Filtering;
using QPP.Filtering.Helpers;
using QPP.Filtering.Exceptions;
using NUnit.Framework;
using System.Collections.Generic;
namespace QPP.Filtering.Tests {
	[TestFixture]
	public class LINQLikeCriteriasTests {
		OperandProperty q = new OperandProperty("q");
		OperandProperty w = new OperandProperty("w");
		OperandProperty e = new OperandProperty("e");
		OperandProperty items = new OperandProperty("Items");
		[Test]
		public void JustQ() {
			Assert.AreEqual("[q]", q.ToString());
		}
		[Test]
		public void QPlusW() {
			Assert.AreEqual("[q] + [w]", (q + w).ToString());
		}
		[Test]
		public void QPlusWMulE() {
			Assert.AreEqual("[q] + [w] * [e]", (q + w * e).ToString());
		}
		[Test]
		public void QMulEPlusWMulE() {
			Assert.AreEqual("[q] * ([e] + [w]) * [e]", (q * (e + w) * e).ToString());
		}
		[Test]
		public void QPlusEMulWPlusE() {
			Assert.AreEqual("[q] + [e] * [w] + [e]", (q + (e * w) + e).ToString());
		}
		[Test]
		public void IsNull() {
			Assert.AreEqual("[w] Is Null", w.IsNull().ToString());
		}
		[Test]
		public void IsNotNull() {
			Assert.AreEqual("[w] Is Not Null", w.IsNotNull().ToString());
		}
		[Test]
		public void Exists() {
			Assert.AreEqual("[Items][]", items[null].ToString());
			Assert.AreEqual("[Items][]", items[null].Exists().ToString());
			Assert.AreEqual("[Items][[q] > [w]]", items[q > w].ToString());
		}
		[Test]
		public void Sum() {
			Assert.AreEqual("[Items][].Sum([w])", items[null].Sum(w).ToString());
			Assert.AreEqual("[Items][[w] > 5].Sum([w])", items[w > 5].Sum(w).ToString());
		}
		[Test]
		public void ImplicitConstCasting() {
			Assert.AreEqual("[q] > 40", (q > 40).ToString());
		}
		[Test]
		public void AndOrTest() {
			Assert.AreEqual("-1 <= [q] && [q] <= 1 || -10 <= [w] && [w] <= 10", CriteriaOperator.ToCStyleString(-1 <= q & q <= 1 | -10 <= w & w <= 10));
		}
	}
	[TestFixture]
	public class CriteriaParserTests {
		[Test]
		public void SimpleOPathGoodTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Name = 'John'");
			Assert.AreEqual("[Name] = 'John'", parseResult.ToString());
			Assert.AreEqual(new BinaryOperator(new OperandProperty("Name"), new OperandValue("John"), BinaryOperatorType.Equal)
				, parseResult);
		}
		[Test, ExpectedException(typeof(CriteriaParserException))]
		public void SimpleOPathBadTest() {
			CriteriaOperator.Parse("Name 'John'");
		}
		[Test]
		public void SimpleCriteriaGenerationTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Name = 'John'");
			Assert.IsNotNull(parseResult);
			BinaryOperator res = parseResult as BinaryOperator;
			Assert.IsNotNull(res);
			Assert.AreEqual("Name", (res.LeftOperand as OperandProperty).PropertyName);
			Assert.AreEqual("John", (res.RightOperand as OperandValue).Value);
		}
		[Test]
		public void AverageCriteriaGenerationTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("(Name = 'John' or Name = 'Bill') and SecondName != 'Doe' and not City like '%cow'");
			Assert.AreEqual("([Name] = 'John' Or [Name] = 'Bill') And [SecondName] <> 'Doe' And [City] Not Like '%cow'", parseResult.ToString());
			Assert.AreEqual(new GroupOperator(GroupOperatorType.And,
				new GroupOperator(GroupOperatorType.Or,
				new BinaryOperator(new OperandProperty("Name"), new OperandValue("John"), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("Name"), new OperandValue("Bill"), BinaryOperatorType.Equal)),
				new BinaryOperator(new OperandProperty("SecondName"), new OperandValue("Doe"), BinaryOperatorType.NotEqual),
				new UnaryOperator(UnaryOperatorType.Not, new BinaryOperator(new OperandProperty("City"), new OperandValue("%cow"), BinaryOperatorType.Like)))
				, parseResult);
		}
		[Test]
		public void SimpleAxisTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Managers[Name like 'Ale%']");
			Assert.AreEqual("[Managers][[Name] Like 'Ale%']", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("Managers"), new BinaryOperator(new OperandProperty("Name"), new OperandValue("Ale%"), BinaryOperatorType.Like))
				, parseResult);
		}
		[Test]
		public void NotLikeTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Name not like 'name%'");
			Assert.AreEqual("[Name] Not Like 'name%'", parseResult.ToString());
			Assert.AreEqual(new UnaryOperator(UnaryOperatorType.Not, new BinaryOperator(new OperandProperty("Name"), new OperandValue("name%"), BinaryOperatorType.Like))
				, parseResult);
		}
		[Test]
		public void NotLikeTest2() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("not Name like 'name%'");
			Assert.AreEqual("[Name] Not Like 'name%'", parseResult.ToString());
			Assert.AreEqual(new UnaryOperator(UnaryOperatorType.Not, new BinaryOperator(new OperandProperty("Name"), new OperandValue("name%"), BinaryOperatorType.Like))
				, parseResult);
		}
		[Test]
		public void CriteriaFromTheOPathOperatorsTest1_1() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Orders[OrderBy = 'John Doe' && Freight = 100]");
			Assert.AreEqual("[Orders][[OrderBy] = 'John Doe' And [Freight] = 100]", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("Orders"), new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("OrderBy"), new OperandValue("John Doe"), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("Freight"), new OperandValue(100), BinaryOperatorType.Equal)))
				, parseResult);
		}
		[Test]
		public void CriteriaFromTheOPathOperatorsTest1_2() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Orders[OrderBy = 'John Doe'] && Orders[Freight = 100]"); 
			Assert.AreEqual("[Orders][[OrderBy] = 'John Doe'] And [Orders][[Freight] = 100]", parseResult.ToString());
			Assert.AreEqual(new GroupOperator(GroupOperatorType.And,
				new ContainsOperator(new OperandProperty("Orders"), new BinaryOperator(new OperandProperty("OrderBy"), new OperandValue("John Doe"), BinaryOperatorType.Equal)),
				new ContainsOperator(new OperandProperty("Orders"), new BinaryOperator(new OperandProperty("Freight"), new OperandValue(100), BinaryOperatorType.Equal)))
				, parseResult);
		}
		[Test, ExpectedException(typeof(CriteriaParserException))]
		public void ContainsTest_E_1() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Orders[q = 'q'].Freight > 100");
			Assert.AreEqual("[Orders][(([q] = 'q') And ([Freight] > 100))]", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("Orders"), new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("q"), new OperandValue("q"), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("Freight"), new OperandValue(100), BinaryOperatorType.Greater)))
				, parseResult);
		}
		[Test, ExpectedException(typeof(CriteriaParserException))]
		public void ContainsTest_E_2() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Orders[q = 'q'].SubOrders[w = 'w']");
			Assert.AreEqual("[Orders][(([q] = 'q') And [SubOrders][([w] = 'w')])]", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("Orders"), new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("q"), new OperandValue("q"), BinaryOperatorType.Equal),
				new ContainsOperator(new OperandProperty("SubOrders"), new BinaryOperator(new OperandProperty("w"), new OperandValue("w"), BinaryOperatorType.Equal))))
				, parseResult);
		}
		[Test, ExpectedException(typeof(CriteriaParserException))]
		public void ContainsTest_E_3() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Orders[q = 'q'].SubOrders[w == 'w'].Freight > 100");
			Assert.AreEqual("[Orders][(([q] = 'q') And [SubOrders][(([w] = 'w') And ([Freight] > 100))])]", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("Orders"), new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("q"), new OperandValue("q"), BinaryOperatorType.Equal),
				new ContainsOperator(new OperandProperty("SubOrders"), new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("w"), new OperandValue("w"), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("Freight"), new OperandValue(100), BinaryOperatorType.Greater)))))
				, parseResult);
		}
		[Test]
		public void ContainsTest_1() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Orders[q = 'q' And Freight > 100]");
			Assert.AreEqual("[Orders][[q] = 'q' And [Freight] > 100]", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("Orders"), new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("q"), new OperandValue("q"), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("Freight"), new OperandValue(100), BinaryOperatorType.Greater)))
				, parseResult);
		}
		[Test]
		public void ContainsTest_2() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Orders[q = 'q' and SubOrders[w = 'w']]");
			Assert.AreEqual("[Orders][[q] = 'q' And [SubOrders][[w] = 'w']]", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("Orders"), new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("q"), new OperandValue("q"), BinaryOperatorType.Equal),
				new ContainsOperator(new OperandProperty("SubOrders"), new BinaryOperator(new OperandProperty("w"), new OperandValue("w"), BinaryOperatorType.Equal))))
				, parseResult);
		}
		[Test]
		public void ContainsTest_3() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Orders[q = 'q' and SubOrders[w == 'w' and Freight > 100]]");
			Assert.AreEqual("[Orders][[q] = 'q' And [SubOrders][[w] = 'w' And [Freight] > 100]]", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("Orders"), new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("q"), new OperandValue("q"), BinaryOperatorType.Equal),
				new ContainsOperator(new OperandProperty("SubOrders"), new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("w"), new OperandValue("w"), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("Freight"), new OperandValue(100), BinaryOperatorType.Greater)))))
				, parseResult);
		}
		[Test]
		public void IsNullTest_1() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("IsNull(column)");
			Assert.AreEqual("[column] Is Null", parseResult.ToString());
			Assert.AreEqual(new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("column"))
				, parseResult);
		}
		[Test]
		public void IsNullTest_2() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("column is null");
			Assert.AreEqual("[column] Is Null", parseResult.ToString());
			Assert.AreEqual(new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("column"))
				, parseResult);
		}
		[Test]
		public void IsNullTest_3() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("column is not null");
			Assert.AreEqual("[column] Is Not Null", parseResult.ToString());
			Assert.AreEqual(new UnaryOperator(UnaryOperatorType.Not, new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("column")))
				, parseResult);
		}
		[Test]
		public void IsNullTest_3a() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("!column is null");
			Assert.AreEqual("[column] Is Not Null", parseResult.ToString());
			Assert.AreEqual(new UnaryOperator(UnaryOperatorType.Not, new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("column")))
				, parseResult);
		}
		[Test, ExpectedException(typeof(CriteriaParserException))]
		public void IsNullTest_E_4() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("isnull(Orders[isnull(q1)].Items[isnull(q2)].q3)");
			Assert.AreEqual("[Orders][(IsNull([q1]) And [Items][(IsNull([q2]) And IsNull([q3]))])]", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("Orders"), new GroupOperator(GroupOperatorType.And,
				new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("q1")),
				new ContainsOperator(new OperandProperty("Items"), new GroupOperator(GroupOperatorType.And,
				new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("q2")),
				new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("q3"))))))
				, parseResult);
		}
		[Test]
		public void IsNullTest_4() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Orders[isnull(q1) and Items[isnull(q2) and isnull(q3)]]");
			Assert.AreEqual("[Orders][[q1] Is Null And [Items][[q2] Is Null And [q3] Is Null]]", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("Orders"), new GroupOperator(GroupOperatorType.And,
				new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("q1")),
				new ContainsOperator(new OperandProperty("Items"), new GroupOperator(GroupOperatorType.And,
				new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("q2")),
				new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("q3"))))))
				, parseResult);
		}
		[Test]
		public void InTest_1() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("qwe.rty.uiop in (1, 's', asd.fgh)");
			Assert.AreEqual("[qwe.rty.uiop] In (1, 's', [asd.fgh])", parseResult.ToString());
			Assert.AreEqual(new InOperator(new OperandProperty("qwe.rty.uiop"), new OperandValue(1), new OperandValue("s"), new OperandProperty("asd.fgh"))
				, parseResult);
		}
		[Test]
		public void InTest_2() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("qwe[isnull(a) and uiop in (1, 's', ^.asd.fgh)]");
			Assert.AreEqual("[qwe][[a] Is Null And [uiop] In (1, 's', [^.asd.fgh])]", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("qwe"), new GroupOperator(GroupOperatorType.And,
				new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("a")),
				new InOperator(new OperandProperty("uiop"), new OperandValue(1), new OperandValue("s"), new OperandProperty("^.asd.fgh"))))
				, parseResult);
		}
		[Test, ExpectedException(typeof(CriteriaParserException))]
		public void InTest_E_2() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("qwe[isnull(a)].uiop in (1, 's', asd.fgh)");
			Assert.AreEqual("[qwe][IsNull([a]) And [uiop] In (1, 's', [^.asd.fgh])]", parseResult.ToString());
			Assert.AreEqual(new ContainsOperator(new OperandProperty("qwe"), new GroupOperator(GroupOperatorType.And,
				new UnaryOperator(UnaryOperatorType.IsNull, new OperandProperty("a")),
				new InOperator(new OperandProperty("uiop"), new OperandValue(1), new OperandValue("s"), new OperandProperty("^.asd.fgh"))))
				, parseResult);
		}
		[Test]
		public void UpSquareTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("qwe.^.rty>3");
			Assert.AreEqual("[qwe.^.rty] > 3", parseResult.ToString());
			Assert.AreEqual(new BinaryOperator(new OperandProperty("qwe.^.rty"), new OperandValue(3), BinaryOperatorType.Greater)
				, parseResult);
		}
		[Test]
		public void RussianSymbolsTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("поле='руссишь'");
			Assert.AreEqual("[поле] = 'руссишь'", parseResult.ToString());
			Assert.AreEqual(new BinaryOperator(new OperandProperty("поле"), new OperandValue("руссишь"), BinaryOperatorType.Equal)
				, parseResult);
		}
		[Test]
		public void ParametersTest() {
			OperandValue[] paramsList;
			CriteriaOperator parseResult = CriteriaOperator.Parse("q > ? and w<? and Orders[e<? and (r=? || r==?)]", out paramsList);
			Assert.AreEqual("[q] > ? And [w] < ? And [Orders][[e] < ? And ([r] = ? Or [r] = ?)]", parseResult.ToString());
			Assert.AreEqual(5, paramsList.Length);
			((OperandValue)paramsList[0]).Value = 0;
			((OperandValue)paramsList[1]).Value = 1;
			((OperandValue)paramsList[2]).Value = 2;
			((OperandValue)paramsList[3]).Value = 3;
			((OperandValue)paramsList[4]).Value = 4;
			Assert.AreEqual("[q] > 0 And [w] < 1 And [Orders][[e] < 2 And ([r] = 3 Or [r] = 4)]", parseResult.ToString());
			Assert.AreEqual(new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("q"), new OperandValue(0), BinaryOperatorType.Greater),
				new BinaryOperator(new OperandProperty("w"), new OperandValue(1), BinaryOperatorType.Less),
				new ContainsOperator(new OperandProperty("Orders"), new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("e"), new OperandValue(2), BinaryOperatorType.Less),
				new GroupOperator(GroupOperatorType.Or,
				new BinaryOperator(new OperandProperty("r"), new OperandValue(3), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("r"), new OperandValue(4), BinaryOperatorType.Equal)))))
				, parseResult);
		}
		[Test]
		public void ConstsTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("string='sval' && int=-1 && float = 1.12e-5 && guid={00000603-0000-0010-8000-00AA006D2EA4} && datetime=#30 Jan 2004# && timespan = #333#");
			Assert.AreEqual("[string] = 'sval' And [int] = -1 And [float] = " + (1.12e-5).ToString(CultureInfo.InvariantCulture) + " And [guid] = {00000603-0000-0010-8000-00aa006d2ea4} And [datetime] = #2004-01-30# And [timespan] = #" + TimeSpan.Parse("333").ToString() + "#", parseResult.ToString());
			Assert.AreEqual(new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("string"), new OperandValue("sval"), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("int"), new OperandValue(-1), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("float"), new OperandValue(1.12e-5), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("guid"), new OperandValue(new Guid("00000603-0000-0010-8000-00AA006D2EA4")), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("datetime"), new OperandValue(new DateTime(2004, 01, 30)), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("timespan"), new OperandValue(new TimeSpan(333, 0, 0, 0)), BinaryOperatorType.Equal))
				, parseResult);
		}
		[Test]
		public void DateTimeConstsTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("f == #28 Jun 2006# || f == #28 Jun 2006 19:28:00# || f == #28 Jun 2006 19:28:00.123#");
			const string expectedResult = "[f] = #2006-06-28# Or [f] = #2006-06-28 19:28:00# Or [f] = #2006-06-28 19:28:00.12300#";
			Assert.AreEqual(expectedResult, parseResult.ToString());
			Assert.AreEqual(expectedResult, CriteriaOperator.Parse(expectedResult).ToString());
		}
		[Test]
		public void OPathExpressionTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("FirstName+'_'+LastName == 'First_Last' && a + - + - b % c > d + --(--e/f)");
			Assert.AreEqual("[FirstName] + '_' + [LastName] = 'First_Last' And [a] + - + - [b] % [c] > [d] + - - (- - [e] / [f])", parseResult.ToString());
			Assert.AreEqual(new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new BinaryOperator(new BinaryOperator(new OperandProperty("FirstName"), new OperandValue("_"), BinaryOperatorType.Plus), new OperandProperty("LastName"), BinaryOperatorType.Plus), new OperandValue("First_Last"), BinaryOperatorType.Equal),
				new BinaryOperator(new BinaryOperator(new OperandProperty("a"), new BinaryOperator(new UnaryOperator(UnaryOperatorType.Minus, new UnaryOperator(UnaryOperatorType.Plus, new UnaryOperator(UnaryOperatorType.Minus, new OperandProperty("b")))), new OperandProperty("c"), BinaryOperatorType.Modulo), BinaryOperatorType.Plus), new BinaryOperator(new OperandProperty("d"), new UnaryOperator(UnaryOperatorType.Minus, new UnaryOperator(UnaryOperatorType.Minus, new BinaryOperator(new UnaryOperator(UnaryOperatorType.Minus, new UnaryOperator(UnaryOperatorType.Minus, new OperandProperty("e"))), new OperandProperty("f"), BinaryOperatorType.Divide))), BinaryOperatorType.Plus), BinaryOperatorType.Greater))
				, parseResult);
		}
		[Test]
		public void OPathExpressionTest2() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("(a+b)*c <> a+(b*c)");
			Assert.AreEqual("([a] + [b]) * [c] <> [a] + [b] * [c]", parseResult.ToString());
		}
		[Test]
		public void OPathXorParentExpressionTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("^.parentproperty^^.parentproperty == 0");
			Assert.AreEqual("[^.parentproperty] ^ [^.parentproperty] = 0", parseResult.ToString());
			Assert.AreEqual(new BinaryOperator(new BinaryOperator(new OperandProperty("^.parentproperty"), new OperandProperty("^.parentproperty"), BinaryOperatorType.BitwiseXor), new OperandValue(0), BinaryOperatorType.Equal)
				, parseResult);
		}
		[Test, ExpectedException(typeof(CriteriaParserException))]
		public void OPathContainsExpressionDocumentedBugTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("Order[Date<#1/1/2005#].Freight + 1 > 6");
			Assert.AreEqual("[Order][([Date] < #1/1/2005# And (([Freight] + 1) > 6))]", parseResult.ToString());
		}
		[Test]
		public void OPathFunctionTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("iif ( a>0,a,-a)>5");
			Assert.AreEqual("Iif([a] > 0, [a], - [a]) > 5", parseResult.ToString());
			Assert.AreEqual(new BinaryOperator(new FunctionOperator(FunctionOperatorType.Iif, new BinaryOperator(new OperandProperty("a"), new OperandValue(0), BinaryOperatorType.Greater), new OperandProperty("a"), new UnaryOperator(UnaryOperatorType.Minus, new OperandProperty("a"))), new OperandValue(5), BinaryOperatorType.Greater)
				, parseResult);
		}
		[Test, ExpectedException(typeof(CriteriaParserException))]
		public void OPathFunctionIncorrectArgsCountTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("iif(1,2,3,4,5) = 3");
		}
		[Test]
		public void DataViewColumnsEscapingTest_1() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("a == @a && a == [a] && a == [\\a]");
			Assert.AreEqual("[a] = [a] And [a] = [a] And [a] = [a]", parseResult.ToString());
			Assert.AreEqual(new GroupOperator(GroupOperatorType.And,
				new BinaryOperator(new OperandProperty("a"), new OperandProperty("a"), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("a"), new OperandProperty("a"), BinaryOperatorType.Equal),
				new BinaryOperator(new OperandProperty("a"), new OperandProperty("a"), BinaryOperatorType.Equal))
				, parseResult);
		}
		[Test]
		public void DataViewColumnsEscapingTest_2() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("a != [ a ]");
			Assert.AreEqual("[a] <> [ a ]", parseResult.ToString());
			Assert.AreEqual(new BinaryOperator(new OperandProperty("a"), new OperandProperty(" a "), BinaryOperatorType.NotEqual)
				, parseResult);
		}
		[Test]
		public void DataViewColumnsBracketEscapingTest() {
			CriteriaOperator parseResult = CriteriaOperator.Parse("[\"\\[\\]!@#$%^&*()_+[\\r\\n\t\\\\] != [ a ]");
			Assert.AreEqual("[\"[\\]!@#$%^&*()_+[\\r\\n\\t\\\\] <> [ a ]", parseResult.ToString());
			Assert.AreEqual(new BinaryOperator(new OperandProperty("\"[]!@#$%^&*()_+[\r\n\t\\"), new OperandProperty(" a "), BinaryOperatorType.NotEqual)
				, parseResult);
		}
		[Test]
		public void ContainsParseContainsUnderNotBugTest() {
			CriteriaOperator oper = CriteriaOperator.Parse("Incidents[1=1]");
			Assert.IsTrue(oper is AggregateOperand);
			UnaryOperator unaryOper = (UnaryOperator)CriteriaOperator.Parse("Not Incidents[1=1]");
			Assert.AreEqual(UnaryOperatorType.Not, unaryOper.OperatorType);
			Assert.IsTrue(unaryOper.Operand is AggregateOperand);
		}
		[Test]
		public void EmptyContainsParseFeatureTest() {
			AggregateOperand oper = (AggregateOperand)CriteriaOperator.Parse("ParentProperty.Property[()]");
			Assert.AreEqual("ParentProperty.Property", oper.CollectionProperty.PropertyName);
			Assert.IsNull(oper.Condition);
			Assert.AreEqual(Aggregate.Exists, oper.AggregateType);
			Assert.IsNull(oper.AggregatedExpression);
		}
		[Test]
		public void EmptyContainsParseFeatureTestFullEmpty() {
			AggregateOperand oper = (AggregateOperand)CriteriaOperator.Parse("ParentProperty.Property[]");
			Assert.AreEqual("ParentProperty.Property", oper.CollectionProperty.PropertyName);
			Assert.IsNull(oper.Condition);
			Assert.AreEqual(Aggregate.Exists, oper.AggregateType);
			Assert.IsNull(oper.AggregatedExpression);
		}
		[Test]
		public void EmptyAggregateContainsParseFeatureTestFullEmpty() {
			AggregateOperand oper = (AggregateOperand)CriteriaOperator.Parse("ParentProperty.Property[].AVG(pointer.value)");
			Assert.AreEqual("ParentProperty.Property", oper.CollectionProperty.PropertyName);
			Assert.AreEqual("pointer.value", ((OperandProperty)oper.AggregatedExpression).PropertyName);
			Assert.AreEqual(Aggregate.Avg, oper.AggregateType);
			Assert.IsNull(oper.Condition);
		}
		void AssertCriteriaToStringAndBack(CriteriaOperator oper, params string[] converts) {
			Assert.IsTrue(converts.Length > 0);
			string currentResult = null;
			foreach(string expected in converts) {
				currentResult = CriteriaOperator.ToString(oper);
				Assert.AreEqual(expected, currentResult);
				oper = CriteriaOperator.Parse(currentResult);
			}
			Assert.AreEqual(currentResult, CriteriaOperator.ToString(oper));
		}
		[Test]
		public void NullPropsToStringAndBackAndToStringContainsOperator() {
			AssertCriteriaToStringAndBack(new ContainsOperator(), "[][]");
		}
		[Test]
		public void NullPropsToStringAndBackAndToStringBetweenOperator() {
			AssertCriteriaToStringAndBack(new BetweenOperator("prop", "minValue", "maxValue"), "[prop] Between('minValue', 'maxValue')");
			AssertCriteriaToStringAndBack(new BetweenOperator(), "() Between((), ())");
		}
		[Test]
		public void NullPropsToStringAndBackAndToStringBinaryOperator() {
			AssertCriteriaToStringAndBack(new BinaryOperator(), "() = ()");
		}
		[Test]
		public void NullPropsToStringAndBackAndToStringUnaryOperator() {
			AssertCriteriaToStringAndBack(new UnaryOperator(), "Not ()");
		}
		[Test]
		public void NullPropsToStringAndBackAndToStringInOperator() {
			AssertCriteriaToStringAndBack(new InOperator(), "() In ()");
			AssertCriteriaToStringAndBack(new InOperator((CriteriaOperator)null, null), "() In ()");
			AssertCriteriaToStringAndBack(new InOperator((CriteriaOperator)null, new CriteriaOperator[] { null, new OperandProperty(), null }), "() In ((), [], ())");
		}
		[Test]
		public void NullPropsToStringAndBackAndToStringGroupOperator() {
			AssertCriteriaToStringAndBack(new GroupOperator(), "()", "");
			AssertCriteriaToStringAndBack(new GroupOperator(GroupOperatorType.And, null, null, null, null), "() And () And () And ()", "");
			AssertCriteriaToStringAndBack(new GroupOperator(GroupOperatorType.Or, null, null, null, null), "() Or () Or () Or ()", "");
		}
		[Test]
		public void NullPropsToStringAndBackAndToStringOperandValue() {
			AssertCriteriaToStringAndBack(new OperandValue("qwe"), "'qwe'");
			AssertCriteriaToStringAndBack(new OperandValue(), "?");
		}
		[Test]
		public void NullPropsToStringAndBackAndToStringOperandProperty() {
			AssertCriteriaToStringAndBack(new OperandProperty(), "[]");
		}
		[Test]
		public void NullPropsToStringAndBackAndToStringAggregateOperand() {
			AssertCriteriaToStringAndBack(new AggregateOperand(), "[][]");
			AssertCriteriaToStringAndBack(new AggregateOperand("prop", Aggregate.Count), "[prop][].Count()");
			AssertCriteriaToStringAndBack(new AggregateOperand("prop", Aggregate.Count, new BinaryOperator("filterProp", 0, BinaryOperatorType.GreaterOrEqual)), "[prop][[filterProp] >= 0].Count()");
		}
		[Test, ExpectedException(typeof(CriteriaParserException))]
		public void NullPropsToStringAndBackAndToStringFunctionOperator_E() {
			AssertCriteriaToStringAndBack(new FunctionOperator(), "None()");
		}
		[Test]
		public void NullPropsToStringAndBackAndToStringFunctionOperator() {
			AssertCriteriaToStringAndBack(new FunctionOperator(FunctionOperatorType.Len, new OperandValue("string")), "Len('string')");
			AssertCriteriaToStringAndBack(new FunctionOperator(FunctionOperatorType.Len, (OperandValue)null), "Len(())");
			AssertCriteriaToStringAndBack(new FunctionOperator(FunctionOperatorType.Substring, new OperandValue("string"), new OperandValue(5)), "Substring('string', 5)");
			AssertCriteriaToStringAndBack(new FunctionOperator(FunctionOperatorType.Substring, new OperandValue("string"), new OperandValue(1), new OperandValue(2)), "Substring('string', 1, 2)");
			AssertCriteriaToStringAndBack(new FunctionOperator(FunctionOperatorType.Substring, new OperandValue("string"), (OperandValue)null), "Substring('string', ())");
			AssertCriteriaToStringAndBack(new FunctionOperator(FunctionOperatorType.Substring, new OperandValue("string"), (OperandValue)null, (OperandValue)null), "Substring('string', (), ())");
		}
		[Test, ExpectedException(typeof(CriteriaParserException))]
		public void ComplexVectorTest() {
			CriteriaOperator op = CriteriaOperator.Parse("Pointer.Collection[q.A+q.B<w.C*w.D].P.O.I * P.Q > L.K * L.W");
		}
		[Test]
		public void TopLevelAggregatesTest() {
			CriteriaOperator op;
			op = CriteriaOperator.Parse("Count");
			Assert.AreEqual(op, new AggregateOperand((OperandProperty)null, null, Aggregate.Count, null));
			op = CriteriaOperator.Parse("Min(Property)");
			Assert.AreEqual(op, new AggregateOperand(null, new OperandProperty("Property"), Aggregate.Min, null));
		}
		[Test]
		public void BugMinusNumber() {
			CriteriaOperator op = CriteriaOperator.Parse("qwe-123");
			Assert.IsTrue(op is BinaryOperator);
		}
		[Test]
		public void ParserErrorTextTest() {
			try {
				CriteriaOperator.Parse("a > b\n or\nqwe > q e");
				Assert.Fail();
			} catch(CriteriaParserException e) {
				Assert.AreEqual("Parser error at line 2, character 8: syntax error; (\"a > b\n or\nqwe > q e\")", e.Message);
			}
		}
		[Test]
		public void ParserErrorTextTestEOF() {
			try {
				CriteriaOperator.Parse("a > b or qwe >");
				Assert.Fail();
			} catch(CriteriaParserException e) {
				Assert.AreEqual("Parser error at line 0, character 14: syntax error; (\"a > b or qwe >{FAILED HERE}\")", e.Message);
			}
		}
		[Test, ExpectedException(typeof(ArgumentException))]
		public void ListInputForSingleCriteriaParseTest() {
			CriteriaOperator.Parse("qwe;rty");
		}
		[Test]
		public void CriteriaOperatorParseListTest() {
			CriteriaOperator[] result = CriteriaOperator.ParseList("qwe;rty + uiop");
			Assert.AreEqual(2, result.Length);
			Assert.AreEqual("[qwe]", result[0].ToString());
			Assert.AreEqual("[rty] + [uiop]", result[1].ToString());
		}
		[Test]
		public void EmptyStringParseTest() {
			Assert.IsNull(CriteriaOperator.Parse(""));
		}
		[Test]
		public void EmptyStringParseListTest() {
			Assert.AreEqual(0, CriteriaOperator.ParseList("").Length);
		}
		[Test]
		public void UpCast() {
			CriteriaOperator op = CriteriaOperator.Parse("[<Bug>Issue]");
			Assert.AreEqual(new OperandProperty("<Bug>Issue"), op);
			op = CriteriaOperator.Parse("<Bug>Issue");
			Assert.AreEqual(new OperandProperty("<Bug>Issue"), op);
			op = CriteriaOperator.Parse("A < 1 and <Bug>Issue = 4");
			Assert.AreEqual(new GroupOperator(new BinaryOperator("A", 1, BinaryOperatorType.Less), new BinaryOperator("<Bug>Issue", 4)), op);
		}
		[Test]
		public void ToStringTestLegacy() {
			CriteriaOperator opa = new OperandValue() & new OperandValue(true) & 'q' & new DateTime(2006, 12, 02) & "q" & 1m & 1d & 1f & new OperandValue((byte)1) & new OperandValue((sbyte)1) & (short)1 & new OperandValue((ushort)1) & 1 & new OperandValue(1u) & 1L & new OperandValue(1uL) & Guid.Empty & TimeSpan.Zero;
			string tostr = opa.LegacyToString();
			Assert.AreEqual("? And True And 'q' And #2006-12-02# And 'q' And 1.0 And 1.0 And 1.0 And 1 And 1 And 1 And 1 And 1 And 1 And 1 And 1 And {00000000-0000-0000-0000-000000000000} And #00:00:00#", tostr);
			Assert.AreNotEqual(opa, CriteriaOperator.Parse(tostr));
		}
		[Test]
		public void ToStringTestNonLegacy() {
			CriteriaOperator opa = new OperandValue() & new OperandValue(true) & 'q' & new DateTime(2006, 12, 02) & "q" & 1m & 1d & 1f & new OperandValue((byte)1) & new OperandValue((sbyte)1) & (short)1 & new OperandValue((ushort)1) & 1 & new OperandValue(1u) & 1L & new OperandValue(1uL) & Guid.Empty & TimeSpan.Zero;
			string tostr = opa.ToString();
			Assert.AreEqual("? And True And 'q'c And #2006-12-02# And 'q' And 1.0m And 1.0 And 1.0f And 1b And 1sb And 1s And 1us And 1 And 1u And 1L And 1uL And {00000000-0000-0000-0000-000000000000} And #00:00:00#", tostr);
			Assert.AreEqual(opa, CriteriaOperator.Parse(tostr));
		}
		[Test]
		public void KeepTypeChar() {
			AssertCriteriaToStringAndBack(new OperandValue('Q'), "'Q'c");
			AssertCriteriaToStringAndBack(new OperandProperty("p") == new OperandValue('Q') & new OperandValue('a') != new OperandValue("a"), "[p] = 'Q'c And 'a'c <> 'a'");
		}
		[Test]
		public void KeepNonFixedPointTypes() {
			AssertCriteriaToStringAndBack(new OperandValue(123.0m) & 123.0 & 123.0f, "123.0m And 123.0 And 123.0f");
		}
		[Test]
		public void KeepFixedPointTypes() {
			AssertCriteriaToStringAndBack(new OperandValue(123) & 123L & new OperandValue(123ul), "123 And 123L And 123uL");
		}
		[Test]
		public void NamedParameterTest() {
			AssertCriteriaToStringAndBack(new OperandProperty("a") > new OperandValue() & new OperandProperty("b") == new OperandParameter("bpar"), "[a] > ? And [b] = ?bpar");
		}
		[Test]
		public void NamedParametersUniqueness() {
			string str = "? > ?1 && ? > ?Two && ? < ?1 && ?1 != ?Two";
			OperandValue[] paramsList;
			CriteriaOperator op = CriteriaOperator.Parse(str, out paramsList);
			Assert.AreEqual(8, paramsList.Length);
			Assert.AreEqual(typeof(OperandValue), paramsList[0].GetType());
			Assert.AreEqual(typeof(OperandParameter), paramsList[1].GetType());
			Assert.AreEqual(typeof(OperandValue), paramsList[2].GetType());
			Assert.AreEqual(typeof(OperandParameter), paramsList[3].GetType());
			Assert.AreEqual(typeof(OperandValue), paramsList[4].GetType());
			Assert.AreEqual(typeof(OperandParameter), paramsList[5].GetType());
			Assert.AreEqual(typeof(OperandParameter), paramsList[6].GetType());
			Assert.AreEqual(typeof(OperandParameter), paramsList[7].GetType());
			Assert.AreNotSame(paramsList[0], paramsList[2]);
			Assert.AreNotSame(paramsList[2], paramsList[4]);
			Assert.AreNotSame(paramsList[0], paramsList[4]);
			Assert.AreEqual("1", ((OperandParameter)paramsList[1]).ParameterName);
			Assert.AreEqual("Two", ((OperandParameter)paramsList[7]).ParameterName);
			Assert.AreSame(paramsList[1], paramsList[5]);
			Assert.AreSame(paramsList[1], paramsList[6]);
			Assert.AreSame(paramsList[3], paramsList[7]);
		}
		[Test]
		public void Bug_B137568_NotNotLike() {
			string str0 = "Not (q not like 'pattern')";
			CriteriaOperator op1 = CriteriaOperator.Parse(str0);
			string str1 = op1.ToString();
			Assert.AreEqual(str1, "Not [q] Not Like 'pattern'");
			CriteriaOperator op2 = CriteriaOperator.Parse(str1);
			string str2 = op2.ToString();
			Assert.AreEqual(str2, "Not [q] Not Like 'pattern'");		
		}
		[Test]
		public void JoinOperandParse() {
			CriteriaOperator co = GroupOperator.And(new BinaryOperator("V", true), new BinaryOperator(new JoinOperand("Products", new BinaryOperator(new OperandProperty("^.State"), new OperandProperty("State"), BinaryOperatorType.Equal), Aggregate.Max, new OperandProperty("V")), new OperandValue(true), BinaryOperatorType.Equal));
			string coString = "V = ? And [<Products>][^.State = State].Max(V) = ?";
			CriteriaOperator coParsed = CriteriaOperator.Parse(coString, true, true);
			Assert.AreEqual(co, coParsed);
		}
		[Test]
		public void JoinOperandToString() {
			CriteriaOperator co = GroupOperator.And(new BinaryOperator("V", true), new BinaryOperator(new JoinOperand("Products", new BinaryOperator(new OperandProperty("^.State"), new OperandProperty("State"), BinaryOperatorType.Equal), Aggregate.Max, new OperandProperty("V")), new OperandValue(true), BinaryOperatorType.Equal));
			string coTestString = "[V] = True And [<Products>][[^.State] = [State]].Max([V]) = True";
			string coString = co.ToString();
			Assert.AreEqual(coTestString, coString);
		}
		[Test]
		public void JoinOperandParseAndBack() {
			string addStringValue = "([V]) = 10";
			string addStringCount = "() = 5";
			string coTempString = "[V] = True And [<Products>][[^.State] = [State]]{0}{1}{2}";
			foreach(Aggregate agr in Enum.GetValues(typeof(Aggregate))) {
				string addString = addStringValue;
				string coTestString;
				if(agr == Aggregate.Exists) {
					coTestString = string.Format(coTempString, string.Empty, string.Empty, string.Empty);
				} else {
					if(agr == Aggregate.Count) addString = addStringCount;
					coTestString = string.Format(coTempString, ".", agr.ToString(), addString);
				}
				CriteriaOperator co = CriteriaOperator.Parse(coTestString);
				string coString = co.ToString();
				Assert.AreEqual(coTestString, coString);
			}
		}
	}
	[TestFixture]
	public class CriteriaLexerTests {
		[Test]
		public void NonClosedColumnTest() {
			try {
				CriteriaOperator.Parse("a > b\n and\nqwe > [qwe");
				Assert.Fail();
			} catch(CriteriaParserException e) {
				Assert.AreEqual("Parser error at line 2, character 6: Malformed property name: missing closing \"]\".; (\"a > b\n and\nqwe > [qwe\")", e.Message);
			}
		}
		[Test]
		public void NonClosedGuidTest() {
			try {
				CriteriaOperator.Parse("{qwerty");
				Assert.Fail();
			} catch(CriteriaParserException e) {
				Assert.AreEqual("Parser error at line 0, character 0: Malformed guid literal: missing closing \"}\".; (\"{FAILED HERE}{qwerty\")", e.Message);
			}
		}
		[Test]
		public void BadGuidTest() {
			try {
				CriteriaOperator.Parse("{qwerty}");
				Assert.Fail();
			} catch(CriteriaParserException e) {
				Assert.AreEqual("Parser error at line 0, character 0: Invalid guid literal value: \"qwerty\".; (\"{FAILED HERE}{qwerty}\")", e.Message);
			}
		}
		[Test]
		public void BadInputCharTest() {
			try {
				CriteriaOperator.Parse("a + б > $");
				Assert.Fail();
			} catch(CriteriaParserException e) {
				Assert.AreEqual("Parser error at line 0, character 8: Invalid input character \"$\".; (\"a + б > {FAILED HERE}$\")", e.Message);
			}
		}
		void TestParseFunctionWrongArgs(string criteria) {
			try {
				CriteriaOperator.Parse(criteria);
				Assert.Fail();
			} catch (CriteriaParserException) { }
		}
		void TestParseFunctionArgs(string criteria, FunctionOperatorType type, CriteriaOperator[] operands) {
			FunctionOperator fo = (FunctionOperator)CriteriaOperator.Parse(criteria);
			Assert.AreEqual(type, fo.OperatorType);
			Assert.AreEqual(operands.Length, fo.Operands.Count);
			for (int i = 0; i < operands.Length; i++) {
				Assert.AreEqual(operands[i], fo.Operands[i]);
			}
		}
		void TestParseFunction(FunctionOperatorType type, int minArgs, int maxArgs) {
			TestParseFunction(type, minArgs, maxArgs, true);
		}
		void TestParseFunction(FunctionOperatorType type, int minArgs, int maxArgs, bool checkWrongArgs) {
			List<CriteriaOperator> operands = new List<CriteriaOperator>();
			string criteria = type.ToString() + "( {0} )";
			string args = string.Empty;
			for (int i = 0; i <= (maxArgs + 1); i++) {
				if (i >= minArgs - 1) {
					if (i < minArgs || i > maxArgs) {
						if(checkWrongArgs)TestParseFunctionWrongArgs(string.Format(criteria, args));
					} else {
						TestParseFunctionArgs(string.Format(criteria, args), type, operands.ToArray());
					}
				}
				if (!string.IsNullOrEmpty(args)) args = args + ", ";
				args = args + i.ToString();
				operands.Add(new OperandValue(i));
			}
		}
		[Test]
		public void FnCharIndex() {
			TestParseFunction(FunctionOperatorType.CharIndex, 2, 4);
		}
		[Test]
		public void FnAbs() {
			TestParseFunction(FunctionOperatorType.Abs, 1, 1);
		}
		[Test]
		public void FnAcos() {
			TestParseFunction(FunctionOperatorType.Acos, 1, 1);
		}
		[Test]
		public void FnAddDays() {
			TestParseFunction(FunctionOperatorType.AddDays, 2, 2);
		}
		[Test]
		public void FnAddHours() {
			TestParseFunction(FunctionOperatorType.AddHours, 2, 2);
		}
		[Test]
		public void FnAddMilliSeconds() {
			TestParseFunction(FunctionOperatorType.AddMilliSeconds, 2, 2);
		}
		[Test]
		public void FnAddMinutes() {
			TestParseFunction(FunctionOperatorType.AddMinutes, 2, 2);
		}
		[Test]
		public void FnAddMonths() {
			TestParseFunction(FunctionOperatorType.AddMonths, 2, 2);
		}
		[Test]
		public void FnAddSeconds() {
			TestParseFunction(FunctionOperatorType.AddSeconds, 2, 2);
		}
		[Test]
		public void FnAddTicks() {
			TestParseFunction(FunctionOperatorType.AddTicks, 2, 2);
		}
		[Test]
		public void FnChar() {
			TestParseFunction(FunctionOperatorType.Char, 1, 1);
		}
		[Test]
		public void FnAddTimeSpan() {
			TestParseFunction(FunctionOperatorType.AddTimeSpan, 2, 2);
		}
		[Test]
		public void FnAddYears() {
			TestParseFunction(FunctionOperatorType.AddYears, 2, 2);
		}
		[Test]
		public void FnAscii() {
			TestParseFunction(FunctionOperatorType.Ascii, 1, 1);
		}
		[Test]
		public void FnAsin() {
			TestParseFunction(FunctionOperatorType.Asin, 1, 1);
		}
		[Test]
		public void FnAtn() {
			TestParseFunction(FunctionOperatorType.Atn, 1, 1);
		}
		[Test]
		public void FnAtn2() {
			TestParseFunction(FunctionOperatorType.Atn2, 2, 2);
		}
		[Test]
		public void FnBigMul() {
			TestParseFunction(FunctionOperatorType.BigMul, 2, 2);
		}
		[Test]
		public void FnCeiling() {
			TestParseFunction(FunctionOperatorType.Ceiling, 1, 1);
		}
		[Test]
		public void FnCos() {
			TestParseFunction(FunctionOperatorType.Cos, 1, 1);
		}
		[Test]
		public void FnCosh() {
			TestParseFunction(FunctionOperatorType.Cosh, 1, 1);
		}
		[Test]
		public void FnExp() {
			TestParseFunction(FunctionOperatorType.Exp, 1, 1);
		}
		[Test]
		public void FnFloor() {
			TestParseFunction(FunctionOperatorType.Floor, 1, 1);
		}
		[Test]
		public void FnGetDate() {
			TestParseFunction(FunctionOperatorType.GetDate, 1, 1);
		}
		[Test]
		public void FnGetDay() {
			TestParseFunction(FunctionOperatorType.GetDay, 1, 1);
		}
		[Test]
		public void FnGetDayOfWeek() {
			TestParseFunction(FunctionOperatorType.GetDayOfWeek, 1, 1);
		}
		[Test]
		public void FnGetDayOfYear() {
			TestParseFunction(FunctionOperatorType.GetDayOfYear, 1, 1);
		}
		[Test]
		public void FnGetHour() {
			TestParseFunction(FunctionOperatorType.GetHour, 1, 1);
		}
		[Test]
		public void FnGetMilliSecond() {
			TestParseFunction(FunctionOperatorType.GetMilliSecond, 1, 1);
		}
		[Test]
		public void FnGetMinute() {
			TestParseFunction(FunctionOperatorType.GetMinute, 1, 1);
		}
		[Test]
		public void FnGetMonth() {
			TestParseFunction(FunctionOperatorType.GetMonth, 1, 1);
		}
		[Test]
		public void FnGetSecond() {
			TestParseFunction(FunctionOperatorType.GetSecond, 1, 1);
		}
		[Test]
		public void FnGetTimeOfDay() {
			TestParseFunction(FunctionOperatorType.GetTimeOfDay, 1, 1);
		}
		[Test]
		public void FnGetYear() {
			TestParseFunction(FunctionOperatorType.GetYear, 1, 1);
		}
		[Test]
		public void FnIif() {
			TestParseFunction(FunctionOperatorType.Iif, 3, 3);
		}
		[Test]
		public void FnInsert() {
			TestParseFunction(FunctionOperatorType.Insert, 3, 3);
		}
		[Test]
		public void FnIsNull() {
			TestParseFunction(FunctionOperatorType.IsNull, 2, 2, false);
		}
		[Test]
		public void FnIsNullOrEmpty() {
			TestParseFunction(FunctionOperatorType.IsNullOrEmpty, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalBeyondThisYear() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalBeyondThisYear, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalEarlierThisMonth() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalEarlierThisMonth, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalEarlierThisWeek() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalEarlierThisWeek, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalEarlierThisYear() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalEarlierThisYear, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalLastWeek() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalLastWeek, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalLaterThisMonth() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalLaterThisMonth, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalLaterThisWeek() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalLaterThisWeek, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalLaterThisYear() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalLaterThisYear, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalNextWeek() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalNextWeek, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalPriorThisYear() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalPriorThisYear, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalToday() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalToday, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalTomorrow() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalTomorrow, 1, 1);
		}
		[Test]
		public void FnIsOutlookIntervalYesterday() {
			TestParseFunction(FunctionOperatorType.IsOutlookIntervalYesterday, 1, 1);
		}
		[Test]
		public void FnLen() {
			TestParseFunction(FunctionOperatorType.Len, 1, 1);
		}
		[Test]
		public void FnLocalDateTimeDayAfterTomorrow() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeDayAfterTomorrow, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeLastWeek() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeLastWeek, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeNextMonth() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeNextMonth, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeNextWeek() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeNextWeek, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeNextYear() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeNextYear, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeNow() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeNow, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeThisMonth() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeThisMonth, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeThisWeek() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeThisWeek, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeThisYear() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeThisYear, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeToday() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeToday, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeTomorrow() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeTomorrow, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeTwoWeeksAway() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeTwoWeeksAway, 0, 0);
		}
		[Test]
		public void FnLocalDateTimeYesterday() {
			TestParseFunction(FunctionOperatorType.LocalDateTimeYesterday, 0, 0);
		}
		[Test]
		public void FnLog() {
			TestParseFunction(FunctionOperatorType.Log, 1, 2);
		}
		[Test]
		public void FnLog10() {
			TestParseFunction(FunctionOperatorType.Log10, 1, 1);
		}
		[Test]
		public void FnLower() {
			TestParseFunction(FunctionOperatorType.Lower, 1, 1);
		}
		[Test]
		public void FnNow() {
			TestParseFunction(FunctionOperatorType.Now, 0, 0);
		}
		[Test]
		public void FnPadLeft() {
			TestParseFunction(FunctionOperatorType.PadLeft, 2, 3);
		}
		[Test]
		public void FnPadRight() {
			TestParseFunction(FunctionOperatorType.PadRight, 2, 3);
		}
		[Test]
		public void FnPower() {
			TestParseFunction(FunctionOperatorType.Power, 2, 2);
		}
		[Test]
		public void FnRemov() {
			TestParseFunction(FunctionOperatorType.Remove, 3, 3);
		}
		[Test]
		public void FnReplace() {
			TestParseFunction(FunctionOperatorType.Replace, 3, 3);
		}
		[Test]
		public void FnReverse() {
			TestParseFunction(FunctionOperatorType.Reverse, 1, 1);
		}
		[Test]
		public void FnRnd() {
			TestParseFunction(FunctionOperatorType.Rnd, 0, 0);
		}
		[Test]
		public void FnRound() {
			TestParseFunction(FunctionOperatorType.Round, 1, 1);
		}
		[Test]
		public void FnSign() {
			TestParseFunction(FunctionOperatorType.Sign, 1, 1);
		}
		[Test]
		public void FnSin() {
			TestParseFunction(FunctionOperatorType.Sin, 1, 1);
		}
		[Test]
		public void FnSinh() {
			TestParseFunction(FunctionOperatorType.Sinh, 1, 1);
		}
		[Test]
		public void FnSqr() {
			TestParseFunction(FunctionOperatorType.Sqr, 1, 1);
		}
		[Test]
		public void FnSubstring() {
			TestParseFunction(FunctionOperatorType.Substring, 2, 3);
		}
		[Test]
		public void FnTan() {
			TestParseFunction(FunctionOperatorType.Tan, 1, 1);
		}
		[Test]
		public void FnTanh() {
			TestParseFunction(FunctionOperatorType.Tanh, 1, 1);
		}
		[Test]
		public void FnToday() {
			TestParseFunction(FunctionOperatorType.Today, 0, 0);
		}
		[Test]
		public void FnToStr() {
			TestParseFunction(FunctionOperatorType.ToStr, 1, 1);
		}
		[Test]
		public void FnTrim() {
			TestParseFunction(FunctionOperatorType.Trim, 1, 1);
		}
		[Test]
		public void FnUpper() {
			TestParseFunction(FunctionOperatorType.Upper, 1, 1);
		}
		[Test]
		public void FnUtcNow() {
			TestParseFunction(FunctionOperatorType.UtcNow, 0, 0);
		}
		[Test]
		public void FnConcat() {
			TestParseFunction(FunctionOperatorType.Concat, 1, 10, false);
		}
		[Test]
		public void FnCustom() {
			TestParseFunction(FunctionOperatorType.Custom, 1, 10, false);
		}
	}
	[TestFixture]
	public class SimpleTests2Xml {
		public void TestCriteriaToXmlSerializerQueryStatement() {
			new System.Xml.Serialization.XmlSerializer(typeof(DevExpress.Xpo.DB.BaseStatement));
			new System.Xml.Serialization.XmlSerializer(typeof(DevExpress.Xpo.DB.SelectStatement));
			new System.Xml.Serialization.XmlSerializer(typeof(DevExpress.Xpo.DB.ModificationStatement));
			new System.Xml.Serialization.XmlSerializer(typeof(DevExpress.Xpo.DB.InsertStatement));
			new System.Xml.Serialization.XmlSerializer(typeof(DevExpress.Xpo.DB.UpdateStatement));
			new System.Xml.Serialization.XmlSerializer(typeof(DevExpress.Xpo.DB.DeleteStatement));
		}
	}
	[TestFixture]
	public class ToStringTests {
		[Test]
		public void NullTest() {
			Assert.AreEqual("", CriteriaOperator.ToString(null));
			Assert.AreEqual("() And ()", CriteriaOperator.ToString(new GroupOperator(null, null)));
			Assert.IsNull(CriteriaOperator.Parse("()"));
			Assert.IsNull(CriteriaOperator.Parse(""));
		}
		[Test]
		public void EnumTest() {
			Assert.AreEqual("'BitwiseXor'", new OperandValue(BinaryOperatorType.BitwiseXor).ToString());
		}
	}
}
#endif
