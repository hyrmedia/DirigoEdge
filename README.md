# DirigoEdge

DirigoEdge is an Open Source development framework built on .Net that makes managing content and building websites faster and easier. It is fully responsive, built on MVC, Microsoft’s CodeFirst, and has a robust shortcode and plugin system.

## Getting Started

1. Clone this repo with `git clone https://github.com/dirigodev/DirigoEdge.git`

2. Edit `web.config` file with your appopriate database connections.
	
3. Set Up Membership provider on your SQL Database:	
	- Run `aspnet_regsql.exe` on your database and follow the on screen instruction. This program installs various sql tables and stored procedures required by the .Net Membership provider.
		- The reg sql program is typically found: C:\Windows\Microsoft.NET\Framework\v4.0.30319    (your .Net version may vary)
	- If you're unfamiliar with `aspnet_regsql.exe` [refer to this walkthrough](http://runtingsproper.blogspot.com/2009/08/using-aspnetregsql-via-command-line-to.html)
	
4. Run the application and create a new user.
	- Running the application for the first time will seed the database and provide you with some initial content to play around with.
	- When you register a new user, you will be added as an Administrator.
	- After you create a user and have successfully logged in, you may add a registration code under Users > Roles.

5. You may need to set permissions on the folders `content/uploaded` and `scripts` so that the 'IISUSR' and 'ISUR' has full permission
	- Edge will automatically combine and minify your CSS and Javascript files in this directory when you publish your solution or run in Release mode
	- More information on SquishIt can be found at https://github.com/jetheredge/SquishIt

# Contributing

## Reporting An Issue

Bugs, feature requests, and other issues can be submitted in [GitHub issues](https://github.com/dirigodev/DirigoEdge/issues).

Before submitting an issue search through open and closed issues to see if your problem has already been resolved or is under discussion.

## Code Contributions

All development work for DirigoEdge is done on the `develop` branch. Any new code contributions should be made on (or branched off) this branch.

### Step 1: Fork
For the project on GitHub and check out your copy locally.

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