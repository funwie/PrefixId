# PrefixId

 ## Stripe like prefix id generator for .Net Core
 ### Generate prefix IDs for C# and .Net core projects

![Tests](https://github.com/funwie/PrefixId/actions/workflows/dotnet_ci.yml/badge.svg)

 ```csharp
 // Implement your domain id
public class TestId : PrefixId<TestId>
{
    public override string Prefix => "test";
}

// Create a new id
var testId = TestId.Create();

testId.Value; // test_tjthidoztiqu5mdjhqr2nh2kzi
testId.ToString(); //test_tjthidoztiqu5mdjhqr2nh2kzi

// TestId from string value
TestId.Parse("test_tjthidoztiqu5mdjhqr2nh2kzi"); // throws if can't parse

var isValid = TestId.TryParse("test_tjthidoztiqu5mdjhqr2nh2kzi", out var testId);

```

