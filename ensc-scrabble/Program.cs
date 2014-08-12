using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace scrabble
{
    class Program
    {

        struct Lettre
        {
            public char caractere;
            public int  valeur;
        };

        struct Joueur
        {
            public string nom;
            public string chevalet;
            public int    score;
            public char[] lettresAPoser;
            public bool   estIA;
        };

        struct Case
        {
            public char         lettre;
            public int          val;
            public TypeCase     type;
            public ConsoleColor couleur;
            public bool         changed;
            public bool         bonus;
        };

        struct Noeud
        {
            public char lettre;
            public Noeud[] fils;
            public bool estMot;
        };

        struct Mot
        {
            public string mot;
            public int x;
            public int y;
            public Orientation ori;
        }

        enum Orientation { HORIZONTAL, VERTICAL };
        enum TypeCase { Normal, Centre, MDouble, MTriple, LDouble, LTriple };

        static Lettre[] alphabet;
        static Noeud[]  dico;
        static char[]   sac;
        static Joueur[] joueurs;
        static Random rnd;

        const string DATA = "../../";
        const char EMPTY_CHAR   = '\0';
        const string version    = "0.8";
        const int CHEVALET_Y    = 23; 
        const int SCREEN_WIDTH  = 60;
        const int SCREEN_HEIGHT = 40;
        const int BOARD_SIZE    = 15;
        const int MENU_OPTION_Y = 29;
        const int MENU_OPTION_H = 9;
        const int FORM_SAISIE_Y = 27;
        const int TAILLE_CHEVALET = 7;

        const ConsoleColor backgroundColor = ConsoleColor.Black;

        static void AfficherIntro()
        {
            Console.Write("\n Scrabble version {0}\n Par Benjamin Denom et Jean-Marc Lacoste\n", version);
            Console.Write("\n [~] Chargement du dictionnaire");
            Console.Write("\n [ ] Initialisation de la pioche");
            Console.Write("\n [ ] Initialisation du plateau");
        }

        static void Main(string[] args)
        {
            InitialiserConsole();

            AfficherIntro();

            bool jeuPret = false;

            /* Initialisation du dictionnaire */
            jeuPret = InitialiserDico(out dico);

            /* Initialisation du sac */
            if(jeuPret)
                jeuPret = InitialiserSac(out sac, out alphabet);

            /* Initialisation de la grille */
            Case[,] plateau = new Case[BOARD_SIZE, BOARD_SIZE];
            if(jeuPret)
                jeuPret = InitialiserPlateau(plateau);

            /* Initialisation des joueurs */
            if(jeuPret)
                InitialiserJoueurs(ref joueurs, ref sac);

            if (!jeuPret)
            {
                WriteAt(1, 8, "Une erreur est survenue lors du chargement du jeu.\n\n Appuyez sur une touche pour quitter...");
            }
            else
            {
                Console.Write("\n Chargement terminé avec succés.\n\n > Jouer   [Entrée]\n > Quitter [Echap ]");
            }

            ConsoleKeyInfo pushedKey = Console.ReadKey();
            AfficherCurseur(false);
            switch(pushedKey.Key){
                case ConsoleKey.Enter:
                    if(jeuPret)
                        Jeu(plateau, joueurs);
                    break;
                default:
                    break;
            }

        }

        /*
         * Génére un "arbre lexical" à partir du fichier contenant tous les mots
         */

        static bool InitialiserDico(out Noeud[] arbre)
        {
            arbre = new Noeud[26];

            try
            {
                StreamReader curseur = new StreamReader(DATA + "dico.txt", System.Text.Encoding.GetEncoding("iso-8859-1"));

                string ligne;
                int nbLignes = 0;
                if ((ligne = curseur.ReadLine()) != null)
                    nbLignes = int.Parse(ligne);

                string[] dico = new string[nbLignes];

                int it = 0;
                while ((ligne = curseur.ReadLine()) != null)
                {
                    if (!ligne.Contains('-') && ligne.Length < 15)
                        dico[it++] = ligne;
                }

                curseur.Dispose();

                /*
                 * A ce stade on dispose de dico[] qui contient tous les mots à mettre dans l'arbre
                 */

                int ir = 0; // indice root (A = 0, B = 1, C = 2, etc.)
                string motCourant = "";
                char derniereLettre = EMPTY_CHAR;
                for (int i = 0; i < dico.Length; i++) // Parcours le dictionnaire
                {
                    motCourant = dico[i];

                    // Changement de lettre de départ (1ere branche)
                    if (motCourant[0] != derniereLettre) // Si on passe d'une premiere lettre a une autre (A->B par exemple)
                    {
                        derniereLettre = motCourant[0];
                        arbre[ir].lettre = derniereLettre;
                        arbre[ir].fils = new Noeud[NombreFils(dico, derniereLettre.ToString(), i)];

                        if (motCourant.Length == 1)
                            arbre[ir].estMot = true;

                        ir++;
                    }


                    char courant;
                    int nbFils;
                    Noeud noeudCourant = arbre[ir - 1];
                    string motif = string.Concat(derniereLettre);

                    for (int j = 1; j < motCourant.Length; j++) // Parcours du mot courant pour l'ajouter dans le graphe
                    {
                        courant = motCourant[j];

                        int k = 0;
                        bool done = false;

                        while (!done && k < noeudCourant.fils.Length) // Parcours des fils du noeud courant
                        {
                            if (noeudCourant.fils[k].lettre == courant) // Fils déjà présent
                            {
                                done = true;
                            }
                            else if (noeudCourant.fils[k].lettre == EMPTY_CHAR) // Fils non présent, place disponible
                            {
                                noeudCourant.fils[k].lettre = courant;

                                // Instanciation des noeuds à venir
                                nbFils = NombreFils(dico, motif + courant, i);
                                if (nbFils != 0)
                                    noeudCourant.fils[k].fils = new Noeud[nbFils];

                                done = true;
                            }

                            if (done)
                            {
                                if (j == motCourant.Length - 1) // Si on est à la fin du mot
                                {
                                    noeudCourant.fils[k].estMot = true; // On marque le noeud comme "formant un mot"
                                }
                                else // Préparation pour le tour suivant
                                {
                                    noeudCourant = noeudCourant.fils[k];
                                    motif = string.Concat(motif, motCourant[j]);
                                }
                            }

                            k++;
                        }
                    }
                }
                WriteAt(2, 4, "v");
                return true;
            }
            catch (Exception e)
            {
                WriteAt(2, 4, "x");
                return false;
            }
        }

        /* 
         * Retourne le nombre de caractéres commun aux deux chaines 'm1' et 'm2' 
         */

        static int EnCommun(string m1, string m2)
        {
            int nb = 0;
            while (nb < m1.Length && nb < m2.Length && m1[nb] == m2[nb])
                nb++;

            return nb;
        }

        /* 
         * Retourne le nombre de fils du noeud correspondant à la derniere lettre de 'motif' 
         * (ie. le nombre de lettre distinctes suivant 'motif' disponibles dans le dictionnaire)
         */

        static int NombreFils(string[] dico, string motif, int start)
        {
            int    nbFils = 0;
            int    i = start;
            bool   exit = false;
            string motCourant = "";
            char   derniereLettre = EMPTY_CHAR;

            while(!exit && i < dico.Length){
                motCourant = dico[i];

                if (motCourant == motif)
                {
                    // pas besoin de compter un fils en plus
                }
                else if (motCourant.Length > motif.Length && motCourant.StartsWith(motif))
                {
                    if (motCourant[motif.Length] != derniereLettre)
                    {
                        derniereLettre = motCourant[motif.Length];
                        nbFils++;
                    }
                }
                else
                {
                    exit = true;
                }

                i++;
            }

            return nbFils;
        }

        /*
         * Initialise le plateau à partir du fichier 'map'
         */

        static bool InitialiserPlateau(Case[,] plateau)
        {
            int w = plateau.GetLength(1);
            int h = plateau.GetLength(0);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    plateau[y, x].couleur = ConsoleColor.White;
                    plateau[y, x].changed = true;
                    plateau[y, x].bonus = true;
                }
            }

            StreamReader curseur = new StreamReader(DATA + "map.txt", System.Text.Encoding.GetEncoding("iso-8859-1"));
            
            string ligne;
            TypeCase type = TypeCase.Normal;
            while ((ligne = curseur.ReadLine()) != null)
            {
                string[] data = ligne.Split('_');
                switch (data[2])
                {
                    case "MD":
                        type = TypeCase.MDouble;
                        break;
                    case "MT":
                        type = TypeCase.MTriple;
                        break;
                    case "LD":
                        type = TypeCase.LDouble;
                        break;
                    case "LT":
                        type = TypeCase.LTriple;
                        break;
                    case "C":
                        type = TypeCase.Centre;
                        break;
                    default:
                        break;
                }
                plateau[int.Parse(data[1]), int.Parse(data[0])].type = type;
            }

            curseur.Dispose();
            WriteAt(2, 6, "v");
            return true;
        }

        static void ValiderCoup(ref Case[,] plateau, string mot, int x, int y, Orientation o, ref Joueur joueurCourant, Case[,] plateauAvant, ref bool premierMot)
        {
            Mot[] motsPrecedents = ExtraireMotsPlateau(plateauAvant);
            Mot[] motsNouveaux = ExtraireMotsPlateau(plateau);
            Mot[] motsModif = RecupererMotsModif(motsNouveaux, motsPrecedents);

            PoserMot(ref plateau, mot, x, y, o, true, true, ref joueurCourant); // Le joueur a validé son coup, on pose le mot sur le plateau d'origine

            if(premierMot)
                motsModif = ExtraireMotsPlateau(plateau);

            /* Mise à jour du score */
            if (motsModif != null)
            {
                int mx = 0, my = 0, dx = 0, dy = 0, estDouble = 0, estTriple = 0, somme = 0;
                foreach (Mot m in motsModif)
                {
                    CalculerDeltas(m.ori, out dx, out dy);
                    mx = m.x;
                    my = m.y;
                    somme = 0;
                    estDouble = 0;
                    estTriple = 0;
                    for (int i = 0; i < m.mot.Length; i++)
                    {
                        int posY = my + i * dy;
                        int posX = mx + i * dx;

                        if (posX < BOARD_SIZE && posY < BOARD_SIZE)
                        {
                            somme += plateau[posY, posX].val * FacteurCase(plateau[posY, posX].type);
                            if ((plateau[posY, posX].type == TypeCase.MDouble || plateau[posY, posX].type == TypeCase.Centre) && plateau[posY, posX].bonus)
                            {
                                plateau[posY, posX].bonus = false;
                                estDouble++;
                            }
                            else if (plateau[posY, posX].type == TypeCase.MTriple && plateau[posY, posX].bonus)
                            {
                                plateau[posY, posX].bonus = false;
                                estTriple++;
                            }
                        }
                    }
                    if (estDouble != 0)
                        somme = somme * (2 * estDouble);
                    if (estTriple != 0)
                        somme = somme * (3 * estTriple);
                    joueurCourant.score += somme;
                }
            }

            if (premierMot)
                premierMot = false;
        }

        static void PositionnerMot(ref Case[,] plateau, ref Joueur joueurCourant, string mot, ref bool premierMot, ref Mot[] motsPrecedents)
        {
            bool estPose = false, estValide = false;
            int x = 0, y = 0;                                              // Position de départ
            Orientation o = Orientation.HORIZONTAL;                        // Orientation de départ

            Case[,] plateauCourant = new Case[BOARD_SIZE, BOARD_SIZE];     // Plateau Temporaire sur lequel positionner le nouveau mot
            Case[,] plateauPrecedent = new Case[BOARD_SIZE, BOARD_SIZE];
            Array.Copy(plateauCourant, plateauPrecedent, plateauCourant.Length);

            AfficherCurseur(false);

            while (!estPose)
            {
                Array.Copy(plateau, plateauCourant, plateau.Length); // On va travailler sur "plateauCourant" temporairement

                estValide = MouvementValide(ref plateauCourant, mot, x, y, o, premierMot, joueurCourant);

                PoserMot(ref plateauCourant, mot, x, y, o, estValide, false, ref joueurCourant); // On pose le mot sur le plateau temporaire sans se soucier de la validité du mouvement (pour l'affichage)
                NotifierChangements(ref plateauCourant, plateauPrecedent);

                AfficherPlateau(plateauCourant);

                // Préparation tour de boucle suivant
                ConsoleKeyInfo pushedKey = AttendreTouche();
                switch (pushedKey.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (y > 0)
                            y--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (y < BOARD_SIZE)
                            y++;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (x > 0)
                            x--;
                        break;
                    case ConsoleKey.RightArrow:
                        if (x < BOARD_SIZE)
                            x++;
                        break;
                    case ConsoleKey.Spacebar:
                        o = (o == Orientation.HORIZONTAL) ? Orientation.VERTICAL : Orientation.HORIZONTAL;
                        break;
                    case ConsoleKey.Enter:
                        if (estValide)
                        {
                            ValiderCoup(ref plateau, mot, x, y, o, ref joueurCourant, plateauPrecedent, ref premierMot);
                            NotifierChangements(ref plateau, plateauCourant);
                            estPose = true;
                        }
                        break;
                    case ConsoleKey.Escape:
                        NotifierChangements(ref plateau, plateauCourant);
                        estPose = true;
                        break;
                    default:
                        break;
                }

                Array.Copy(plateauCourant, plateauPrecedent, plateauCourant.Length);
            }
            MasquerSaisie();
        }

        static string DemanderMot()
        {
            /* Mise à jour de l'affichage */
            MasquerMenuActions();

            /* Préparation du curseur */
            Console.CursorSize = 100;
            AfficherCurseur(true);

            Write("---------------------------------------", 0, 33, true);
            Write(" AIDE ", 0, 33, true);
            Console.SetCursorPosition(0, 35);
            Console.Write(" 1. Entrez un mot disponible dans le dictionnaire.\n 2. Déplacez le mot jusqu'à ce qu'il apparaisse en vert.\n 3. Validez votre coup en appuyant sur [Entrée].\n (*) Vous pouvez quitter votre tour avec un mot vide.");

            string mot = "";
            bool exit = false;
            /* Boucle permettant forcement un mot existant */
            while(!exit)
            {
                AfficherSaisie();
                Console.SetCursorPosition(18, 29);
                mot = Console.ReadLine().ToUpper();

                if (MotExiste(mot) || mot.Length == 0)
                {
                    exit = true;
                }
                else
                {
                    SetCursorColor(ConsoleColor.Red, ConsoleColor.Black);
                    Write("Réessayez le mot n'existe pas", 0, 31, true);
                    SetCursorColor(ConsoleColor.White, ConsoleColor.Black);
                }
            }

            return mot;
        }

        /*
         * Boucle de jeu
         */

        static string[] GenererMots(char[] lettres)
        {
            string[] propositions = new string[500];
            int p = 0;
            string motCourant;
            char lettreCourante;
            for (int i = 0; i < lettres.Length; i++) // Fixe la 1ere lettre
            {
                motCourant = "";
                lettreCourante = lettres[i];
                int n = 0;
                bool found = false;
                while (!found && n < dico.Length) // Trouve le point d'entrée
                {
                    if (dico[n].lettre == lettreCourante || lettreCourante == '-')
                        found = true;

                    if (!found)
                        n++;
                }

                if (found)
                {
                    Noeud noeud = dico[n];
                    motCourant = motCourant = string.Concat(motCourant, lettreCourante);
                    char[] lettres_tmp = SupprLettre(lettres, Array.IndexOf(lettres, lettreCourante));
                    chercherMatch(noeud, lettres_tmp, motCourant, propositions, ref p);
                }
            }

            return propositions;
        }

        static char[] SupprLettre(char[] lettres, int indice)
        {
            char[] tmp = new char[lettres.Length - 1];
            int j = 0;
            for (int i = 0; i < lettres.Length; i++)
            {
                if (i != indice)
                    tmp[j++] = lettres[i];
            }

            return tmp;
        }

        static void chercherMatch(Noeud noeudCourant, char[] lettres, string motCourant, string[] propositions, ref int p)
        {
            if (noeudCourant.fils == null || p >= 499)
                return;

            for (int f = 0; f < noeudCourant.fils.Length; f++)
            {
                for (int l = 0; l < lettres.Length; l++)
                {
                    if (noeudCourant.fils[f].lettre == lettres[l])
                    {
                        motCourant = string.Concat(motCourant, noeudCourant.fils[f].lettre);

                        if (noeudCourant.fils[f].estMot && !propositions.Contains(motCourant))
                        {
                            propositions[p++] = motCourant;
                        }

                        char[] lettres_tmp = SupprLettre(lettres, l);
                        chercherMatch(noeudCourant.fils[f], lettres_tmp, motCourant, propositions, ref p);
                        motCourant = motCourant.Substring(0, motCourant.Length - 1);
                    }
                }
            }
        }

        static void AfficherFin(Joueur[] joueurs)
        {
            Console.Clear();
            Console.WriteLine("\n La partie est terminée ! ");
            for (int i = 0; i < joueurs.Length; i++)
            {
                Console.WriteLine("    " + joueurs[i].nom + " a " + joueurs[i].score + " points.");
            }
            Console.WriteLine("\n Appuyez sur une touche pour quitter...");
        }

        static char[] FusionnerTabChar(char[] t1, char[] t2)
        {
            char[] tmp = new char[t1.Length + t2.Length];

            int i = 0;
            for (int j = 0; j < t1.Length; j++)
            {
                if(t1[j] != EMPTY_CHAR)
                    tmp[i++] = t1[j];
            }

            for (int j = 0; j < t2.Length; j++)
            {
                if (t2[j] != EMPTY_CHAR)
                    tmp[i++] = t2[j];
            }

            char[] tmp2 = new char[i];
            for (int j = 0; j < tmp2.Length; j++)
            {
                tmp2[j] = tmp[j];
            }

            return tmp2;
        }

        static char[] ExtraireLettrePlateau(Case[,] plateau)
        {
            int hauteur = plateau.GetLength(0);
            int largeur = plateau.GetLength(1);

            char[] tmp = new char[hauteur*largeur];

            int k=0;
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    tmp[k++] = plateau[i, j].lettre;
                }
            }

            return tmp;
        }

        static bool FinPartie(Joueur[] joueurs)
        {
            for (int i = 0; i < joueurs.Length; i++)
            {
                if (joueurs[i].chevalet.Equals(""))
                    return true;
            }
            return false;
        }

        static void Jeu(Case[,] plateau, Joueur[] joueurs)
        {
            /* Gestion joueur */
            int idJoueur = 0;
            Joueur joueurCourant;

            /* Initialisation */
            int nbTours     = 0;
            string mot      = "";
            bool premierMot = true;

            /* Liste mots */
            Mot[] motsPrecedents = null;

            Console.Clear();
            ConsoleKeyInfo lastKey = new ConsoleKeyInfo();
            while (!FinPartie(joueurs)) // Boucle de jeu
            {
                joueurCourant = joueurs[idJoueur];

                if (joueurCourant.estIA)
                {
                    AfficherPlateau(plateau);
                    AfficherChevalet(joueurCourant);
                    Clear(0, MENU_OPTION_Y - 1, SCREEN_WIDTH, SCREEN_HEIGHT - MENU_OPTION_Y);
                    Write(joueurCourant.nom + " réfléchit...", 0, CHEVALET_Y + 7, true);

                    Case[,] plateauAvant = new Case[BOARD_SIZE, BOARD_SIZE];
                    Array.Copy(plateau, plateauAvant, plateau.Length);

                    string[] propositions = null; 
                    bool estValide = false;

                    int x = 0, y = 0;
                    Orientation ori = Orientation.HORIZONTAL;

                    if (premierMot)
                    {
                        propositions = GenererMots(joueurCourant.chevalet.ToCharArray());
                        x = 7;
                        y = 7;
                        int i = 0;
                        while (!estValide)
                        {
                            if (propositions[i] == null)
                                break;

                            estValide = MouvementValide(ref plateau, propositions[i], x, y, ori, premierMot, joueurCourant);

                            if (estValide)
                            {
                                mot = propositions[i];
                            }

                            i++;
                        }
                    }
                    else
                    {
                        int hauteur = plateau.GetLength(0);
                        int largeur = plateau.GetLength(1);
                        int posLettre = 0;

                        for (int l = 0; l < hauteur; l++)
                        {
                            for (int c = 0; c < largeur; c++)
                            {
                                if (plateau[l, c].lettre != EMPTY_CHAR)
                                {
                                    char[] lettres = joueurCourant.chevalet.ToCharArray();
                                    AjouterElement(ref lettres, plateau[l, c].lettre);
                                    propositions = GenererMots(lettres);
                                    for (int i = 0; i < propositions.Length; i++)
                                    {
                                        if (propositions[i] == null)
                                            break;

                                        posLettre = propositions[i].IndexOf(plateau[l, c].lettre);
                                        if (posLettre != -1)
                                        {
                                            // Horizontalement
                                            x = c - posLettre;
                                            y = l;
                                            ori = Orientation.HORIZONTAL;

                                            estValide = MouvementValide(ref plateau, propositions[i], c - posLettre, l, Orientation.HORIZONTAL, false, joueurCourant);

                                            if (!estValide)
                                            {
                                                x = c;
                                                y = l - posLettre;
                                                ori = Orientation.VERTICAL;

                                                estValide = MouvementValide(ref plateau, propositions[i], c, l - posLettre, Orientation.VERTICAL, false, joueurCourant);
                                            }

                                            if (estValide)
                                            {
                                                mot = propositions[i];
                                                c = 50;
                                                l = 50;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    ValiderCoup(ref plateau, mot, x, y, ori, ref joueurCourant, plateauAvant, ref premierMot);
                    AfficherPlateau(plateau);
                    Write(joueurCourant.nom + " a terminé son tour.", 0, CHEVALET_Y + 7, true);
                    Write("Appuyez sur une touche pour continuer...", 0, CHEVALET_Y + 8, true);
                    AttendreTouche();
                }
                else // Joueur humain
                {
                    AfficherPlateau(plateau);
                    AfficherChevalet(joueurCourant);
                    AfficherMenuActions();

                    bool aide = false;
                    int choix = 0;
                    do
                    {

                        lastKey = AttendreTouche();
                        switch (lastKey.Key)
                        {
                            case ConsoleKey.A:
                                mot = DemanderMot();
                                if (mot.Length != 0)
                                    choix = 1;
                                else
                                {
                                    choix = 3;
                                }
                                break;
                            case ConsoleKey.Z:
                                choix = 2;
                                break;
                            case ConsoleKey.E:
                                choix = 3;
                                break;
                            case ConsoleKey.R:
                                aide = !aide;
                                AfficherAide(aide);
                                choix = 0;
                                break;
                            default:
                                choix = 0;
                                break;
                        }
                    } while (choix == 0);

                    /* CHOIX POSITIONNEMENT D'UN MOT */
                    if (choix == 1)
                    {
                        PositionnerMot(ref plateau, ref joueurCourant, mot, ref premierMot, ref motsPrecedents);
                    }
                    else if (choix == 2)
                    {
                        MasquerMenuActions();
                        AfficherSaisie();
                        Write("------ AIDE ------", 0, SCREEN_HEIGHT - 4, true);
                        Write("Saisissez les lettres que vous voulez changer.", 0, SCREEN_HEIGHT - 3, true);
                        Write("------------------", 0, SCREEN_HEIGHT - 2, true);

                        Console.CursorSize = 100;
                        AfficherCurseur(true);
                        Console.SetCursorPosition(18, FORM_SAISIE_Y + 2);

                        string lettres;
                        do
                        {
                            lettres = Console.ReadLine().ToUpper();
                        } while (lettres.Length > 7);


                        for (int i = 0; i < lettres.Length; i++)
                        {
                            RendreLettre(lettres[i], ref joueurCourant.chevalet);
                            joueurCourant.chevalet += PiocherLettre();
                        }

                        MasquerSaisie();
                        AfficherCurseur(false);
                    }
                }

                CopierJoueur(joueurCourant, ref joueurs[idJoueur]);
                idJoueur = (idJoueur + 1) % joueurs.Length;
                nbTours++;
            }
            AfficherFin(joueurs);
            AttendreTouche();
        }

        static void CopierJoueur(Joueur src, ref Joueur dst)
        {
            dst.chevalet = src.chevalet;
            dst.score    = src.score;
        }

        /*
         * Compare l'état du plateau courant avec celui d'avant pour savoir quelles sont les cases à redessiner
         */

        static void NotifierChangements(ref Case[,] plateauCourant, Case[,] plateauPrecedent)
        {
            int largeur = plateauCourant.GetLength(1);
            int hauteur = plateauCourant.GetLength(0);

            /* maj des etats */
            for (int i = 0; i < hauteur; i++)
            {
                for (int j = 0; j < largeur; j++)
                {
                    if (plateauCourant[i, j].lettre != plateauPrecedent[i, j].lettre || plateauCourant[i, j].couleur != plateauPrecedent[i, j].couleur)
                        plateauCourant[i, j].changed = true;
                }
            }
        }

        /*
         * Détermine si un mouvement est valide ou non
         *    mot         : mot qu'on veut placer
         *    x, y        : position du mot
         *    orientation : orientation du mot
         *    premierMot  : booléen déterminant si le mot doit être positionné au centre ou non
         */

        static bool MouvementValide(ref Case[,] plateau, string mot, int x, int y, Orientation orientation, bool premierMot, Joueur joueur)
        {
            int largeur = plateau.GetLength(1);
            int hauteur = plateau.GetLength(0);

            Case[,] pTmp = new Case[hauteur, largeur];
            Array.Copy(plateau, pTmp, plateau.Length);

            int dx, dy;
            CalculerDeltas(orientation, out dx, out dy);

            if (!EstSurPlateau(x, y, dx, dy, mot.Length, largeur, hauteur)) // 1. Vérifier que le mot entre intégralement sur le plateau
                return false;

            int posX, posY, j = 0;
            char[] aPoser = new char[7];
            int milieu = BOARD_SIZE / 2;
            bool adjacent = false, estMilieu = false;
            for (int i = 0; i < mot.Length; i++)
            {
                posX = x + i * dx;
                posY = y + i * dy;

                if (pTmp[posY, posX].lettre != EMPTY_CHAR && pTmp[posY, posX].lettre != mot[i]) // L'intersection ne se fait pas sur une lettre commune
                    return false;

                if (pTmp[posY, posX].lettre == EMPTY_CHAR)
                {
                    if (j == 7) // On a déjà 7 lettres à poser il ne pourra donc pas former le mot
                        return false;

                    aPoser[j] = mot[i];
                    j++;
                }

                if (adjacent == false && EstAdjacent(plateau, posX, posY)) // on utilise plateau pour ne pas avoir de proximité avec les autres lettres du mot
                    adjacent = true;

                if (posY == milieu && posX == milieu)
                    estMilieu = true;

                pTmp[posY, posX].lettre = mot[i]; // On pose la lettre courante
            }

            if (premierMot && !estMilieu)
                return false;

            if (!adjacent && !premierMot)
                return false;

            if (!VerifierChevalet(aPoser, joueur.chevalet))
                return false;
            else
                Array.Copy(aPoser, joueur.lettresAPoser, aPoser.Length);

            if (!PlateauValide(pTmp))
                return false;

            plateau = pTmp;
            return true;
        }

        static bool VerifierChevalet(char[] proposition, string chevalet)
        {
            char[] chevaletTmp = chevalet.ToCharArray();
            int index = 0;
            if (proposition.Length > chevalet.Length)
                return false;
            
            else
            {
                for (int i = 0; i < proposition.Length; i++)
                {
                    if ((index = Array.IndexOf(chevaletTmp, proposition[i])) == -1)
                    {
                        if ((index = Array.IndexOf(chevaletTmp, '-')) == -1)
                            return false;
                    }

                    chevaletTmp[index] = EMPTY_CHAR;
                }
            }

            return true;
        }

        /*
         * Procédure permettant de poser un 'mot' sur le 'plateau'
         * fixer : booléen permettant de savoir si on est en mode positionnement (position finale non déterminée)
         */

        static void PoserMot(ref Case[,] plateau, string mot, int x, int y, Orientation orientation, bool estValide, bool fixer, ref Joueur joueur)
        {
            int largeur = plateau.GetLength(1);
            int hauteur = plateau.GetLength(0);

            int dx, dy;
            CalculerDeltas(orientation, out dx, out dy);

            int posX, posY;
            for (int i = 0; i < mot.Length; i++)
            {
                posX = x + i * dx;
                posY = y + i * dy;

                if(EstSurPlateau(posX, posY, largeur, hauteur))
                {
                    plateau[posY, posX].lettre = mot[i]; // On pose la lettre courante
                    plateau[posY, posX].val = ScoreLettre(mot[i]);
                    plateau[posY, posX].changed = true;
                    if (fixer)
                    {
                        plateau[posY, posX].couleur = ConsoleColor.White;

                        /* Mise à jour du chevalet */
                        int index;
                        char[] charChevalet = joueur.chevalet.ToCharArray();
                        for (int k = 0; k < joueur.lettresAPoser.Length; k++)
                        {
                            index = Array.IndexOf(charChevalet, joueur.lettresAPoser[k]);

                            if (index == -1) // On cherche éventuellement le joker
                                index = Array.IndexOf(charChevalet, '-');

                            if (index != -1)
                            {
                                charChevalet[index] = PiocherLettre();
                            }
                        }
                        joueur.chevalet = new String(charChevalet);
                    }
                    else if (estValide)
                        plateau[posY, posX].couleur = ConsoleColor.Green;
                    else
                        plateau[posY, posX].couleur = ConsoleColor.Red;
                }
            }
        }



        static Mot[] RecupererMotsModif(Mot[] mots, Mot[] motsPrecedents)
        {
            Mot[] motsModif = null;

            if (mots == null)
                return null;

            for (int i = 0; i < mots.Length; i++)
            {
                bool found = false;
                int j = 0;
                while (motsPrecedents != null && !found && j < motsPrecedents.Length)
                {
                    if (motsPrecedents[j].mot.Equals(mots[i].mot))
                        found = true;

                    j++;
                }
                if (!found)
                {
                    AjouterMot(ref motsModif, mots[i].mot, mots[i].x, mots[i].y, mots[i].ori);
                }
            }

            /*
            int i = 0;
            while (motsPrecedents != null && mots != null && i < motsPrecedents.Length && i < mots.Length)
            {
                if (!mots[i].mot.Equals(motsPrecedents[i].mot))
                    AjouterMot(ref motsModif, mots[i].mot, mots[i].x, mots[i].y, mots[i].ori);

                i++;
            }
            while (mots != null && i < mots.Length)
            {
                AjouterMot(ref motsModif, mots[i].mot, mots[i].x, mots[i].y, mots[i].ori);
                i++;
            }*/

            return motsModif;
        }

        /*
         * Retourne le score d'une 'lettre'
         */

        static int ScoreLettre(char lettre)
        {
            for (int i = 0; i < alphabet.Length; i++)
            {
                if (alphabet[i].caractere == lettre)
                    return alphabet[i].valeur;
            }

            return 0;
        }

        /*
         * Retourne le facteur bonus d'un type de case 'tc'
         */

        static int FacteurCase(TypeCase tc)
        {
            int facteur = 1;
            switch (tc)
	        {
                case TypeCase.LDouble:
                    facteur = 2;
                 break;
                case TypeCase.LTriple:
                    facteur = 3;
                 break;
                default:
                    facteur = 1;
                 break;
            }

            return facteur;
        }

        /*
         * Détermine si une case a au moins une lettre dans ses voisins
         */

        static bool EstAdjacent(Case[,] p, int x, int y)
        {
            try
            {
                if (p[y, x - 1].lettre != EMPTY_CHAR || p[y, x + 1].lettre != EMPTY_CHAR || p[y - 1, x].lettre != EMPTY_CHAR || p[y + 1, x].lettre != EMPTY_CHAR)
                    return true;
            }
            catch(IndexOutOfRangeException e)
            {
                //Console.WriteLine(e.Message);
            }
            return false;
        }

        /*
         * Retourne un booléen permettant de savoir si tous les mots du plateau existent ou non
         */

        static Mot[] ExtraireMotsPlateau(Case[,] p)
        {
            int x = 0, y = 0, lastX = 0, lastY = 0;
            int largeur = p.GetLength(1);
            int hauteur = p.GetLength(0);

            Mot[] mots = null;

            // Parcours largeur
            string motCourant;
            while (y < hauteur)
            {
                x = 0;
                motCourant = String.Empty;
                while (x < largeur)
                {
                    if (p[y, x].lettre != EMPTY_CHAR)
                    {
                        if (lastX == 0 && lastY == 0)
                        {
                            lastX = x;
                            lastY = y;
                        }

                        motCourant = string.Concat(motCourant, p[y, x].lettre);
                    }
                    else
                    {
                        if (motCourant.Length > 1)
                        {
                            bool existe = MotExiste(motCourant);
                            if (motCourant.Length > 1 && !existe)
                            {
                                return null;
                            }
                            else if (motCourant.Length > 1 && existe)
                            {
                                AjouterMot(ref mots, motCourant, lastX, lastY, Orientation.HORIZONTAL);
                            }
                        }
                        motCourant = String.Empty;
                        lastX = 0;
                        lastY = 0;
                    }
                    x++;
                }
                y++;
            }

            // Parcours hauteur
            x = 0;
            while (x < largeur)
            {
                y = 0;
                motCourant = String.Empty;
                while (y < hauteur)
                {
                    if (p[y, x].lettre != EMPTY_CHAR)
                    {
                        motCourant = string.Concat(motCourant, p[y, x].lettre);
                    }
                    else
                    {
                        if (motCourant.Length > 1)
                        {
                            bool existe = MotExiste(motCourant);
                            if (motCourant.Length > 1 && !existe)
                            {
                                return null;
                            }
                            else if (motCourant.Length > 1 && existe)
                            {
                                AjouterMot(ref mots, motCourant, x, y, Orientation.VERTICAL);
                            }
                        }
                        motCourant = String.Empty;
                    }
                    y++;
                }
                x++;
            }

            return mots;
        }

        static bool PlateauValide(Case[,] p)
        {
            Mot[] mots = ExtraireMotsPlateau(p);

            return mots != null;
        }

        static void AjouterMot(ref Mot[] tab, string mot, int x, int y, Orientation o)
        {
            if (tab == null)
            {
                tab = new Mot[1];
                tab[0].mot = mot;
                tab[0].x = x;
                tab[0].y = y;
                tab[0].ori = o;
            }
            else
            {
                Mot[] tmp = new Mot[tab.Length + 1];
                for (int i = 0; i < tab.Length; i++)
                {
                    tmp[i] = tab[i];
                }
                tmp[tab.Length].mot = mot;
                tmp[tab.Length].x = x;
                tmp[tab.Length].y = y;
                tmp[tab.Length].ori = o;

                tab = tmp;
            }
        }

        /*
         * Parcourt l'arbre pour déterminer si un mot existe ou non
         */

        static bool MotExiste(string mot) // TODO : break -> while
        {
            char lettre;
            bool exit = false;
            Noeud noeud = new Noeud();
            for (int i = 0; i < mot.Length; i++)
            {
                lettre = mot[i];

                if (i == 0)
                {
                    for (int j = 0; j < dico.Length; j++)
                    {
                        if (dico[j].lettre == lettre)
                        {
                            if (mot.Length == 1)
                                return true;

                            noeud = dico[j];
                            break;
                        }
                    }
                }
                else
                {
                    int j = 0;
                    while (j <= noeud.fils.Length)
                    {
                        if (j == noeud.fils.Length)
                            return false;

                        if (noeud.fils[j].lettre > lettre) // On a dépassé la lettre recherchée donc le mot n'existe pas
                            return false;

                        if (noeud.fils[j].lettre == lettre)
                        {
                            if (i == mot.Length - 1 && noeud.fils[j].estMot)
                                return true;

                            noeud = noeud.fils[j]; // On avance dans l'arbre

                            if (noeud.fils == null) // Si le nouveau noeud n'a pas de fils on ne pourra pas aller plus loin
                                return false;

                            break;
                        }
                        j++;
                    }
                }
            }

            return false;
        }

        /*
         * Détermine si un segment (vertical ou horizontal) entre intégralement sur le plateau
         */

        static bool EstSurPlateau(int x, int y, int dx, int dy, int mLongueur, int pLargeur, int pHauteur)
        {
            return !(x < 0 || y < 0 || (x + dx * (mLongueur-1)) >= pLargeur || (y + dy * (mLongueur-1)) >= pHauteur);
        }

        /* 
         * Détermine si un point est sur le plateau
         */

        static bool EstSurPlateau(int x, int y, int pLargeur, int pHauteur)
        {
            return !(x < 0 || y < 0 || x >= pLargeur || y >= pHauteur); 
        }

        /*
         * Permet de définir le sens d'une évolution sur x et y à partir d'une orientation
         *   sur x -> x = 1, y = 0 (horizontale)
         *   sur y -> x = 0, y = 1 (verticale)
         */

        static void CalculerDeltas(Orientation orientation, out int dx, out int dy)
        {
            dx = 0;
            dy = 0;

            switch (orientation)
            {
                case Orientation.HORIZONTAL:
                    dx = 1;
                    break;
                case Orientation.VERTICAL:
                    dy = 1;
                    break;
                default:
                    break;
            }
        }

        /* 
         * Permet d'écrire un texte à une position sur la console
         *     'center' : centre le texte sur sa ligne
         */

        static void Write(string text, int x, int y, bool center) /* FIXME : permettre la saisie d'une chaine contenant \n */
        {
            if (center)
                Console.SetCursorPosition(SCREEN_WIDTH / 2 - text.Length / 2, y);
            else
                Console.SetCursorPosition(x, y);

            Console.Write(text);
        }

        static void AfficherTitre()
        {
            SetCursorColor(ConsoleColor.White, ConsoleColor.DarkRed);
            Write("                               ", 0, 1, true);
            Write("        S C R A B B L E        ", 0, 2, true);
            Write("                               ", 0, 3, true);
        }

        /*
         * Permet de dessiner le plateau
         */

        static void AfficherPlateau(Case[,] p)
        {
            AfficherTitre();

            int largeur = p.GetLength(1);
            int hauteur = p.GetLength(0);

            int grille_gauche = (SCREEN_WIDTH / 2) - largeur;
            int grille_haut   = 5;

            Console.SetCursorPosition(grille_gauche, grille_haut);
            SetCursorColor(ConsoleColor.White, backgroundColor);

            // Dessin cadre superieur
            for (int i = 0; i < largeur*2+1; i++)
            {
                if (i == 0)
                    Console.Write('┌');
                else if (i == largeur * 2)
                {
                    Console.Write('┐');
                }
                else
                {
                    Console.Write('─');
                }
            }
            Console.SetCursorPosition(grille_gauche, (1 + grille_haut)); // Dessin grille
            for (int y = 0; y < hauteur; y++)
            {
                Console.Write("│");
                for (int x = 0; x < largeur; x++)
                {
                    if (p[y, x].changed)
                    {
                        Console.SetCursorPosition((grille_gauche + 1) + x * 2, (1 + grille_haut) + y);
                        SetCursorColor(p[y, x].couleur, CouleurCase(p[y, x].type));
                        Console.Write(p[y, x].lettre);
                        SetCursorColor(ConsoleColor.White, backgroundColor);
                        p[y, x].changed = false;
                    }
                }
                Console.SetCursorPosition(grille_gauche + largeur * 2, (1 + grille_haut) + y);
                Console.Write("│");
                Console.SetCursorPosition(grille_gauche, (1 + grille_haut) + (y + 1));
            }
            // Dessin cadre inferieur
            Console.SetCursorPosition(grille_gauche, (1 + grille_haut) + hauteur);
            for (int i = 0; i < largeur * 2 + 1; i++)
            {
                if (i == 0)
                    Console.Write('└');
                else if (i == largeur * 2)
                {
                    Console.Write('┘');
                }
                else
                {
                    Console.Write('─');
                }
            }
            Console.Write(Environment.NewLine);
        }

        static ConsoleKeyInfo AttendreTouche()
        {
            Console.SetCursorPosition(0, 0);        // positionne le curseur au début
            ConsoleKeyInfo key = Console.ReadKey(); // mais comme la touche écrit
            Console.SetCursorPosition(0, 0);        // on recule
            Console.Write(" ");                     // pour l'effacer

            return key;
        }

        static void AfficherChevalet(Joueur j)
        {
            string chevalet = "";
            for (int i = 0; i < j.chevalet.Length; i++)
            {
                chevalet += j.chevalet[i];
                if (i != j.chevalet.Length - 1)
                    chevalet += " ";
            }
            Clear(0, CHEVALET_Y + 1, SCREEN_WIDTH, 1);
            Clear(0, CHEVALET_Y + 3, SCREEN_WIDTH, 1);
            Write("┌─────────────────────────────┐", 0, CHEVALET_Y, true);
            Write("│", 15, CHEVALET_Y+1, false);
            Write(j.nom + " (" + j.score + " pts)", 0, CHEVALET_Y+1, true);
            Write("│", 45, CHEVALET_Y+1, false);
            Write("├─────────────────────────────┤", 0, CHEVALET_Y+2, true);
            Write("│", 15, CHEVALET_Y+3, false);
            Write(chevalet, 0, CHEVALET_Y+3, true);
            Write("│", 45, CHEVALET_Y+3, false);
            Write("└─────────────────────────────┘", 0, CHEVALET_Y+4, true);
        }

        static void AfficherMenuActions()
        {
            Clear(0, MENU_OPTION_Y - 1, SCREEN_WIDTH, SCREEN_HEIGHT - MENU_OPTION_Y);
            Write("─────────────────────────────", 0, MENU_OPTION_Y, true);
            Write("            MENU             ", 0, MENU_OPTION_Y + 1, true);
            Write("─────────────────────────────", 0, MENU_OPTION_Y + 2, true);
            Write(" Poser un mot            [A]", 0, MENU_OPTION_Y+3, true);
            Write(" Changer des lettres     [Z]", 0, MENU_OPTION_Y+4, true);
            Write(" Passer son tour         [E]", 0, MENU_OPTION_Y+5, true);
            Write("─────────────────────────────", 0, MENU_OPTION_Y+6, true);
            Write(" Afficher l'aide         [R]", 0, MENU_OPTION_Y+7, true);
            Write("─────────────────────────────", 0, MENU_OPTION_Y+8, true);
            AfficherCurseur(false);
        }

        static void MasquerMenuActions()
        {
            Clear(0, MENU_OPTION_Y, SCREEN_WIDTH, MENU_OPTION_H);
        }

        static void AfficherSaisie()
        {
            Write("MOT", 0, FORM_SAISIE_Y + 1, true);
            Write("┌─────────────────────────────┐", 0, FORM_SAISIE_Y + 1, true);
            Write("│░                           ░│", 0, FORM_SAISIE_Y + 2, true);
            Write("└─────────────────────────────┘", 0, FORM_SAISIE_Y + 3, true);
        }

        static void MasquerSaisie()
        {
            Clear(0, FORM_SAISIE_Y, SCREEN_WIDTH, 3);
        }

        static void AfficherAide(bool afficher)
        {
            if (afficher)
            {
                MasquerMenuActions();
                Console.SetCursorPosition(0, MENU_OPTION_Y + 1);
                Console.Write(" Si vous pensez pouvoir poser un mot appuyez sur [A].\n Vous pourrez alors le saisir et le poser sur le plateau.\n\n Pour changer des lettres de votre chevalet appuyez sur [Z].\n Sinon vous pouvez passer votre tour en appuyant sur [E].\n\n Attention, annuler une action revient a passer son tour.");
            }
            else
            {
                AfficherMenuActions();
            }
        }

        /*
         * Retourne la couleur d'une case à partir de son type 't'
         */

        static ConsoleColor CouleurCase(TypeCase t)
        {
            ConsoleColor c;

            switch (t)
            {
                case TypeCase.Normal:
                    c = backgroundColor;
                    break;
                case TypeCase.Centre:
                    c = ConsoleColor.DarkMagenta;
                    break;
                case TypeCase.MDouble:
                    c = ConsoleColor.DarkMagenta;
                    break;
                case TypeCase.MTriple:
                    c = ConsoleColor.DarkRed;
                    break;
                case TypeCase.LDouble:
                    c = ConsoleColor.DarkBlue;
                    break;
                case TypeCase.LTriple:
                    c = ConsoleColor.DarkCyan;
                    break;
                default:
                    c = ConsoleColor.DarkGray;
                    break;
            }

            return c;
        }

        /*
         * Procédure qui initialise la pioche à partir du fichier 'lettres' contenant une liste des lettres sous la forme :
         *     LETTRE_NOMBRE_SCORE
         */

        static bool InitialiserSac(out char[] sac, out Lettre[] alphabet)
        {
            StreamReader curseur = new StreamReader(DATA + "lettres.txt", System.Text.Encoding.GetEncoding("iso-8859-1"));

            string ligne;
            int nbLignes = 0;
            if ((ligne = curseur.ReadLine()) != null)
                nbLignes = int.Parse(ligne);

            alphabet = new Lettre[nbLignes];

            int[] occurences = new int[nbLignes];
            int nbPiecesCourant = 0;
            int nbPiecesTotal   = 0;

            int j = 0;
            while ((ligne = curseur.ReadLine()) != null && j < nbLignes)
            {
                string[] args = ligne.Split('_');

                /* On ajoute la nouvelle lettre à notre alphabet */
                if (args.Length >= 2)
                    InitialiserLettre(ref alphabet[j], char.Parse(args[0]), int.Parse(args[1]));

                /* On met a jour le tableau d'occurences */
                nbPiecesCourant = int.Parse(args[2]);
                occurences[j++] = nbPiecesCourant;

                nbPiecesTotal += nbPiecesCourant;
            }

            curseur.Dispose();

            /* On peut remplir le sac grâce au tableau d'occurences */
            int l = 0;
            sac = new char[nbPiecesTotal];
            for (int i = 0; i < alphabet.Length; i++)
            {
                for (int k = 0; k < occurences[i]; k++)
                {
                    sac[l++] = alphabet[i].caractere;
                }
            }
            WriteAt(2, 5, "v");
            return true;
        }

        /* 
         * Procédure servant de constructeur à la structure lettre
         */

        static void InitialiserLettre(ref Lettre lettre, char caractere, int valeur)
        {
            lettre.caractere = caractere;
            lettre.valeur = valeur;
        }

        /*
         * Procédure qui initialise les joueurs
         */
        
        static void InitialiserJoueurs(ref Joueur[] joueurs, ref char[] sac)
        {
            Console.WriteLine("\n");
            int nbJoueurs;
            do
            {
                Console.Write(" Nombre de joueurs (entre 2 et 4) ? ");
            } while (!int.TryParse(Console.ReadLine(), out nbJoueurs) || nbJoueurs < 0 || nbJoueurs > 4);
            joueurs = new Joueur[nbJoueurs];

            for (int i = 0; i < joueurs.Length; i++)
            {
                string nom = "";
                do{
                    Console.Write("\n Nom du joueur n°{0} ? ", i + 1);
                    nom = Console.ReadLine();
                }while(nom.Length < 1 || nom.Length > 12);
                joueurs[i].nom = nom;

                Console.Write(" Est-ce un joueur humain ? (Oui/Non) ", i + 1);
                joueurs[i].estIA = (Console.ReadLine().ToUpper() == "OUI") ? false : true;
                joueurs[i].chevalet = "";
                joueurs[i].score = 0;
                joueurs[i].lettresAPoser = new char[TAILLE_CHEVALET];

                for (int j = 0; j < TAILLE_CHEVALET; j++)
                {
                    joueurs[i].chevalet += PiocherLettre();
                }
            }
        }

        /*
         * Fonction qui retourne une lettre après l'avoir retirée de la pioche
         */

        static char PiocherLettre()
        {
            if (sac.Length == 0)
                return EMPTY_CHAR;
            int indice = rnd.Next() % sac.Length;
            char lettre = sac[indice];

            RetirerElement(ref sac, indice);

            return lettre;
        }

        static void RendreLettre(char lettre, ref string chevalet)
        {
            char[] charChevalet = chevalet.ToCharArray();
            RetirerElement(ref charChevalet, Array.IndexOf(charChevalet, lettre));

            chevalet = "";
            for (int i = 0; i < charChevalet.Length; i++)
            {
                chevalet = String.Concat(chevalet, charChevalet[i]);
            }

            AjouterElement(ref sac, lettre);
            Console.WriteLine();
        }

        static void RetirerElement(ref char[] tab, int indice)
        {
            if (indice == -1)
                return;

            char[] tmp = new char[tab.Length - 1];

            int i = 0;
            for (int j = 0; j < tab.Length; j++)
            {
                if (j != indice)
                {
                    tmp[i] = tab[j];
                    i++;
                }
            }

            tab = tmp;
        }

        static void AjouterElement(ref char[] tab, char elem)
        {
            char[] tmp = new char[tab.Length + 1];
            Array.Copy(tab, tmp, tab.Length);
            tmp[tab.Length] = elem;

            tab = tmp;
        }

        /*
         * Procédure qui initialise la console
         */

        static void InitialiserConsole()
        {
            Console.Title = "Scrabble v" + version;
            Console.SetWindowSize(SCREEN_WIDTH, SCREEN_HEIGHT);
            Console.SetBufferSize(SCREEN_WIDTH, SCREEN_HEIGHT);
            SetCursorColor(ConsoleColor.White, ConsoleColor.Black);
            AfficherCurseur(false);

            rnd = new Random();
        }

        /* 
         * Procédure qui change la couleur du curseur
         */

        static void SetCursorColor(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        static void WriteAt(int x, int y, string texte)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(texte);
        }

        /* 
         * Procédure qui efface un rectangle (x, y, w, h) de la console 
         */

        static void Clear(int x, int y, int w, int h)
        {
            for (int cy = y; cy < (y+h); cy++)
            {
                Console.SetCursorPosition(x, cy);
                for (int cx = x; cx < (x+w); cx++)
                {
                    Console.Write(" ");
                }
            }
        }

        /* 
         * Procédure qui affiche ou masque le curseur en fonction de 'b'
         *    b vrai -> affiche le curseur
         *    b faux -> masque le curseur
         */

        static void AfficherCurseur(bool b)
        {
            Console.CursorVisible = b;
        }
    }
}
