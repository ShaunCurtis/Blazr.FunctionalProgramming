# Functional Programming in C#

This series of articles is about applying *Functional Programming* principles to C#.

Let's start with a reality check.

> You won't switch from an *OOP* to a *FP* programming paradigm in a day, a month, or even a year.  If you continue using C# you'll never switch totally.

So why start?  What's the point?

The final judgement is yours, but read these articles first.  Don't be put off by what you've read so far.

And, though you may not realise it, you're already doing some functional style programming:

```csharp
var items = invoice.Items.Where(item => item.IsLive);
```

is an *FP* LINQ query.  

I can only relate my experience.  The road was a little rocky.  I made more than one false start.  But *FP*  has helped me solve several fundimental code problems in a more concise and elegant way.  Few methods are longer than ten lines.  Many are static.  It made me think a lot more about *state*, immutability, and aggregates.  My code contains few bugs now, it's better tested, and there's a lot less of it!

If you're here, you've probably read a few articles already. This one isn't top of the search list.  You've encountered the *Monad Enigma*.

To quote *Douglas Crockford*:

> The monadic curse is that once someone learns what monads are and how to use them, they lose the ability to explain them to other people.

The problem is, that at some point on page one, the author throws you in a the deep end.  The
 article quickly becomes incomprehensible.  It often contains a few gems, but those are buried by a mass of otherwise incomprehensible alpha numeric characters.

Here's a classic piece of mis-information you don't need to know:

> A Monad is just a Monoid in the Category of EndofFunctors.

Wipe it from your memory, you don't need to understand it.  I'm not trying to impress here: I have no idea what it means either!

And then there's the **Monad** word.  It dominates conversation.  Everyone wants to explain it before covering the basics.  It doesn't stand alone.  In coding terms, *Monads* and *Functors* exist in a context.  You need to understand the context first.

That's it with the M word (and all the jargon) for a while.  

Let's head to the shallow end and *containers*.

