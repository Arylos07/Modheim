# Contributing

## Workflow
I welcome pull requests from everyone. By contributing to this project, you agree to abide by the Modheim [code of conduct]. You also agree that by submitting a pull request for this project, **[your contribution will be licensed under the MIT license for this project][modheim license]**.

Note: For large changes or added features, please open an Issue indicating what you are intended to work on. This is to allow greater collaboration and prevent multiple isolated indivduals accidentially working on the same thing.

1. Fork and clone the Modheim repo (see [fork a repo] )
2. Switch to the develop branch.
3. Make your change. Add tests and/or demo scenes for your change. 
4. Push to your fork and [submit a pull request][pr].

I will try to give initial feedback to pull requests within a few days. However, this may not always be possible. Please be patient, I will look at every pull request as soon as I can.

Your pull request will have a better chance of being accepted if you do the following: 

* Send one pull request for each new feature. It's time consuming for us to review multi-feature changes.
* Write tests for each change / new feature (not always possible)
* Write a [good commit message][commit].
* Target and make changes against the develop branch.

## How to make changes to Modheim.
Modheim uses Unity 2021.1.11f1 as its environment. This is because I'm a great backend programmer, but horrible with UI. Since this is a side project meant for fun, I kept it simple by using an environment I know in and out. The code is still standard C# but using Unity objects and UI will use the Unity UI system.

So, make sure you have that version of Unity installed. Also, ensure you are using .NET 2.0. The project settings should ensure it stays that way, but if you get strange errors, like "class ZipFile not found", the .NET version may have been changed to 4.X.

### Why .NET 2.0?
Because the IO system Modheim uses relies on features in 2.0. 4.X has a more robust system, but it's not included and will need to be added as a reference and Unity don't like that. And the libraries I could get working make the workflow of packaging files more difficult. (and I'm too lazy to try and rewrite the workflow). 

### Why aren't you using Unity UI Toolkit?
Because that's still brand new and, to be fair, I only saw it a week before starting Modheim. My main Unity project uses 2019 so I haven't really followed the new updates. But the benefit of FOSS is that someone who is a much better UI programmer than me can do it.

### Why is the UI design so bad?
Because, again, I'm a great programmer but horrible UI designer. The only thing I know is dark colours are great and contrast all colours. And make sure the layout is consistent. Again, the benefit of FOSS is that someone who is a much better UI designer can do it. Unity's UI system is pretty simple, if counterintuitive at times so even non-programmers can make Modheim's UI cleaner.

[code of conduct]: https://github.com/Arylos07/Modheim/blob/master/CODE_OF_CONDUCT.md
[commit]: http://chris.beams.io/posts/git-commit/
[fork a repo]: https://help.github.com/articles/fork-a-repo/
[Modheim license]: https://github.com/Arylos07/Modheim/blob/master/LICENSE
[pr]: https://github.com/Arylos07/Modheim/compare
