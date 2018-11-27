# A1

[![Build Status][build-badge]][builds]
[![NuGet][nuget-badge]][nuget-pkg]
[![MyGet][myget-badge]][edge-pkgs]

A1 is a [.NET Standard][netstd] 1.0 library whose purpose is to help parse and
format A1-style cell references (or addresses) like `$FOO$42`, such as those
typically found in spreadsheet programs such as [Microsoft Excel][xl].

## Usage

Convert a column reference expressed as an _alphabet-string_ (one or more
characters from the alphabet) into a zero-based column offset:

```c#
var a = A1Convert.AlphaColumnNumber("A");
Console.WriteLine(a); // 0
```

Convert a zero-based column offset into an alphabet-string:

```c#
var foo = A1Convert.NumberColumnAlpha(4461)
Console.WriteLine(foo); // FOO
```

Query maximum column offset or alphabet-string supported:

```c#
Console.WriteLine(A1Convert.MaxColumn); // 16384
Console.WriteLine(A1Convert.NumberColumnAlpha(A1Convert.MaxColumn)); // XFD
```

There are four value types defined in the library:

- `Row`: represents a row number where first row is 1.
- `Col`: represents a column number where first column is 1.
- `RowCol`: represents a `Row` and `Col` pair.
- `Address`: builds on `RowCol` and permits flagging the row and/or column as
  being absolute or relative. It also provides parsing A1-style cell
  references into an `Address` instance.

Following is a demonstration of to use these types.

Initialize a `RowCol` then format in A1-style:

```c#
var rc = new Col(13) + new Row(35);
Console.WriteLine(rc);                            // (35,13)
Console.WriteLine(rc.FormatA1());                 // M35
```

`RowCol` values can be compared for equality and inequality:

```c#
Console.WriteLine(rc == new Col(35) + new Row(13)); // False
Console.WriteLine(rc != new Col(35) + new Row(13)); // True
Console.WriteLine(rc == new Col(13) + new Row(35)); // True
Console.WriteLine(rc != new Col(13) + new Row(35)); // False
```

A `RowCol` value can be tested for containment within a matrix defined by
a pair of `RowCol` values (top-left then bottom-right):

```c#
var xy = new Col(13) + new Row(35);
var lt = new Col(1) + new Row(1); // or default(RowCol)
var rbSmall = new Col(10) + new Row(10);
var rbLarge = new Col(100) + new Row(100);
Console.WriteLine(xy.IsContainedIn(lt, rbSmall)); // False
Console.WriteLine(xy.IsContainedIn(lt, rbLarge)); // True
```

Determine offset to another `RowCol`:

```c#
var a = new Col(17) + new Row(5);
var b = new Col(19) + new Row(71);
var o = a.OffsetTo(b, (y, x) => new { X = x, Y = y });
Console.WriteLine(o.ToString()); // { X = 2, Y = 66 }
```

Determine size of the box formed by two `RowCol` values:

```c#
var a = new Col(17) + new Row(5);
var b = new Col(19) + new Row(71);
var s = a.Size(b, (h, w) => new { Width = w, Height = h });
Console.WriteLine(s.ToString()); // { X = 3, Y = 67 }
```

Initializing an `Address` and formatting:


```c#
Console.WriteLine(new Address(new Col(13) + new Row(35)));                               // M35
Console.WriteLine(new Address(new Col(13) + new Row(35), AddressTraits.Absolute));       // $M$35
Console.WriteLine(new Address(new Col(13) + new Row(35), AddressTraits.AbsoluteColumn)); // $M35
Console.WriteLine(new Address(new Col(13) + new Row(35), AddressTraits.AbsoluteRow));    // M$35
```

Parsing an A1-style reference into an `Address`:

```c#
var a = Address.ParseA1("M$35");
Console.WriteLine(a);           // M$35
Console.WriteLine(a.IsRowAbs);  // True
Console.WriteLine(a.Row);       // 35
Console.WriteLine(a.IsColAbs);  // True
Console.WriteLine(a.Col);       // 13
Console.WriteLine(a.RowCol);    // (35,13)
```

Make an `Address` absolute and then relative again:

```c#
var m35 = Address.ParseA1("M35");
var abs = m35.MakeAbsolute();       // $M$35
Console.WriteLine(abs);
var rel = abs.MakeRelative();       // M35
Console.WriteLine(rel);
```

Parse a range into an `Address` couple (i.e. `Tuple<Address, Address>`):

```c#
var range = Address.ParseA1Range("B5:Z10");
Console.WriteLine(range); // (B5, Z10)
```

Parse a range into an anonymous type holding the `Address` couple:

```c#
var range = Address.ParseA1Range("B5:Z10", (a, b) => new { From = a, To = b });
Console.WriteLine(range.ToString()); // (B5, Z10)
```

`Address` values can be compared for equality and inequality:

```c#
var range = Address.ParseA1Range("B5:Z10");
Console.WriteLine(range.Item1 == range.Item2); // False
Console.WriteLine(range.Item1 != range.Item2); // True
```


  [netstd]: https://docs.microsoft.com/en-us/dotnet/articles/standard/library
  [xl]: https://www.microsoft.com/excel
  [build-badge]: https://img.shields.io/appveyor/ci/raboof/a1.svg
  [myget-badge]: https://img.shields.io/myget/raboof/v/A1.svg?label=myget
  [edge-pkgs]: https://www.myget.org/feed/raboof/package/nuget/A1
  [nuget-badge]: https://img.shields.io/nuget/v/A1.svg
  [nuget-pkg]: https://www.nuget.org/packages/A1
  [builds]: https://ci.appveyor.com/project/raboof/a1
