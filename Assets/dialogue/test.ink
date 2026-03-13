VAR item_name = ""

=== present_item ===
{ item_name:
    - "sword":
        Le PNJ examine la lame... Où as-tu trouvé ça ?
        -> end
    - "scroll":
        Ce parchemin ! Je le croyais perdu depuis des années !
        -> end
    - "staff":
        Un bâton de magicien ? Tu pratiques la magie ?
        -> end
    - else:
        Je ne sais pas quoi penser de cet objet.
        -> end
}

=== end ===
-> END