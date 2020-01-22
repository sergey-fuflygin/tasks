select *
from Article
--Id	Subject
--1		Pushkin
--2		Lermontov
--3		Lomonosov
--4		Pifagor
--5		Mendeleev

select *
from Tag
--Id	Name
--1		Literature
--2		Math
--3		Music

select *
from ArticleTag
--Id	Article_Id	Tag_Id
--1		1			1
--2		2			1
--3		3			1
--4		3			2
--5		4			2

select a.Subject, t.Name
from Article a
	left outer join ArticleTag m on a.Id = m.Article_Id
	left outer join Tag t on m.Tag_Id = t.Id
--Subject	Name
--Pushkin	Literature
--Lermontov	Literature
--Lomonosov	Literature
--Lomonosov	Math
--Pifagor	Math
--Mendeleev	NULL