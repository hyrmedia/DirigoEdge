# DirigoEdge

[![Join the chat at https://gitter.im/dirigodev/DirigoEdge](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dirigodev/DirigoEdge?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

DirigoEdge is an Open Source CMS development framework built on ASP.Net C# that makes managing content and building websites faster and easier. It is fully responsive, built on MVC, Microsoft’s CodeFirst, and has a robust shortcode and plugin system.

Explore DirigoEdge functions on our [YouTube channel](https://www.youtube.com/channel/UCf2zaGBp4ifFMeu0tetutYw)

## Getting Started

See the **[getting started guide](https://github.com/dirigodev/DirigoEdge/wiki/Getting-Started)** in the Wiki.

## DirigoEdgeCore

DirigoEdge and its plugins depend on [DirigoEdgeCore](https://github.com/dirigodev/DirigoEdgeCore). Stable versions are available and will be automatically installed from NuGet. If you would like to work with the latest development build of Core you'll need to pull down the project.

### Step 1: Clone the repo
Clone the repo from GitHub. This will automatically pull down the `develop` branch

```
$ git clone https://github.com/dirigodev/DirigoEdgeCore.git
```

### Step 2: Build the project
Once you've opened the project in Visual Studio you'll need to do a build. This will automatically generate a new NuGet package in  `DirigoEdgeCore/bin`.

### Step 3: Add a new NuGet Source
In Package Manager Settings go to **NuGet Package Manager > Package Sources**. Add a new source and point it to `C:\path\to\DirigoEdgeCore\bin`. Make sure you hit Update when you're done.

You can change the order in which NuGet looks for packages here, or you can explicitly set it in the Package Manager Console.

### Step 4: Install DirigoEdgeCore
Now you're ready to install DirigoEdgeCore in the Package Manager Console. If you didn't set your local package source at the top of list, be sure to set **Package source** to your local source.

Now you are ready to update the package. The `-Reinstall` flag is necessary if you have made changes to DirigoEdgeCore and haven't changed the version in the nuspec file.
```
update-package DirigoEdgeCore -Reinstall
```

# Contributing

## Reporting An Issue

Bugs, feature requests, and other issues can be submitted in [GitHub issues](https://github.com/dirigodev/DirigoEdge/issues).

Before submitting an issue search through open and closed issues to see if your problem has already been resolved or is under discussion.

## Code Contributions

All development work for DirigoEdge is done on the `develop` branch. Any new code contributions should be made on (or branched off) this branch.

### Step 1: Fork
Fork the project on GitHub and check out your copy locally.

```
$ git clone https://github.com/dirigodev/DirigoEdge.git
$ cd DirigoEdge
$ git remote add upstream git://github.com/dirigodev/DirigoEdge.git
```

### Step 2: Branch

Create a new branch for your feature or bug fix

```
$ git checkout -b my-feature-branch -t origin/develop
```

### Step 3: Commit

Try to keep your commit history clean. Your commit messages should concisely describe all the change contained within the commit.

If you have a lot of small commits, consider [squashing your commits](http://davidwalsh.name/squash-commits-git).

### Step 4: Rebase

Pull requests with merge conflicts will not be accepted. Rebasing your fork from time to time while you're working on it will prevent major headaches when you're ready to submit your PR.

Use `git rebase` (not `git merge`) to sync your work from time to time.

```
$ git fetch upstream
$ git rebase upstream/develop
```

### Step 5: Push

```
$ git push origin my-feature-branch
```

Go to [https://github.com/yourusername/DirigoEdge](https://github.com/yourusername/DirigoEdge) and select your feature branch. Click the 'Pull Request' button and fill out the form.

Changes will be reviewed by the DirigoEdge core team. If there are comments to address, apply your changes in a separate commit and push that to your feature branch. This will automatically update your pull request.
