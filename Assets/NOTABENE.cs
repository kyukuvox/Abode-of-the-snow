// !!!TOUS LES ITEMS DOIVENT AVOIR LE SCRIPT ITEMPICKUP!!!
// !!!TOUS LES PNJS DOIVENT AVOIR LE SCRIPT NPCWITHITEMDIALOGUE!!!


//POUR AJOUTER UN NOUVELLE ITEM
//
// OBJETS --> Create --> Inventory --> Item
//
// Crée un item et donne lui ses sprits et la description qui vas s'y afficher 
//
// Dans l'inspecteur de l'item dans la scene --> glisser déposer l'item dans le script ItemPickup
//
// Dans l'inspecteur des pnjs lié à l'item --> donner l'item dans Items dialogues avec le dialogue lié
//
// !!Sauvegarde des Data!! --> Scripts --> SAUVEGARDE --> DATABASE --> ItemDataBase --> donner l'item dans l'inspecteur


//POUR AJOUTER UN NOUVEAU DIALOGUE
//
// DIALOGUES --> Create --> Dialogue --> DialogueData
//
// Donner:   TriggerCardGame oui / non 
// Le nom du NPC lié 
// Sprite du Joueur et du NPC lié
// BadDecision oui / non
// Lignes de dialogue lié à l'objet avec ce personnage
// TriggerCardGame oui / non  --> Si oui --> donner cartes d'information de jeu du joueur et du NPC --> choisir l'item que le joueur recevra en récompense 
//
// Dans l'inspecteur du pnj lié --> donner le dialogue dans Items dialogues avec l'item lié


//POUR AJOUTER UNE NOUVELLE CARTE DE JEU 
//
// Card MiniGame --> Card --> Create --> CardGame --> Card 
//
// Donne informations lié à la carte 
//
// Ajouter la cartes dans un deck ou donner la possibilité de le gagner
//
// Pour ajouter dans un deck --> Card MiniGame --> CharacterCard --> choisir le NPC voulu --> donner comme carte de récompense après un combat 
//
// !!Sauvegarde des Data!! --> Scripts --> SAUVEGARDE --> DATABASE --> CardDataBase --> donner l'item dans l'inspecteur

//POUR AJOUTER UN NPC DE COMBAT
//
// Card MiniGame --> CharacterCard --> Create --> CardGame --> CharacterCard
//
// Donner: 
// Nom + Sprite du personnage 
// Vie + Max de points d'action + points d'actions récupéré par tours 
// Choisir un deck de départ
// Carte de récompense quand il es vaincu 
//
// Ajouter la CharacterCard dans le dialogue du PNJ lié quand cela trigger un combat 