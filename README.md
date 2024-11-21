# Benutzer-Dokumentation
> Übersicht über die Sprachbefehle
>
> Aktion | Befehl
> -------|-------
> Teleportieren | Teleport, Los
> Drehen (90°)  | Links / Rechts
> Objekt fallen lassen | Drop
## Starten der Anwendung in der CAVE
### Starten der CAVE
Zum starten der CAVE müssen alle Beamer mithilfe des ```CaveOn```-Scrips in ```C:\HTWCave``` eingeschaltet werden
### Ausführen der Anwendung
Die Anwendung befindet sich standartmäßig im Build-Ordner des Projektverzeichnisses. Vorgefertigt befindet sich die Anwendung in ```C:\HTWCave\UnityCave\DEMO\Build``` . Führen Sie diese aus.

### Beamer-Konfiguration laden
Sind die Bildschirme falsch angeordnet, müssen die Beamer neu kalibriert werden oder es muss eine funktionierende Kalibration geladen werden. Starte hierbei, während die Anwendug läuft das Kalibrations-Tool und konfiguriere die Beamer entsprechend. Meist reicht es die letzte Konfiguration zu laden.
**Schließe die Kalibrations-Software und starte die Demo-Anwendung neu.** Ohne Neustart kann nicht auf das Mikrofon zugegriffen werden. 

### Steuerung
Die Interaktion erfolgt in erster Linie mit Spracherkennung
#### Lokomotion
Wenn der rechte Arm gestreckt ist erscheint ein blauer Laserstrahl. Das ist der Teleportations-Pointer. Zum teleportieren muss der Strahl auf den Boden gerichtet sein und es muss das Sprachkommando ```Teleport``` oder ```Los``` gesagt werden. Alternativ wird die Teleportation auch ausgelöst, wenn die ```Linke Hand den Kopf berührt``` . 

Da die CAVE auf der Rückseite keine projektion hat, kann mit den Befehlen ```Links``` und ```Rechts``` die Kamera um jeweils 90 Grad gedreht werden. Es ist empfehlenswert die vordere Leinwand immer als Sichtziel zu haben, da sich hier auch die Kinect befindet und das Tracking so am stabilsten ist.

#### Greifen und Loslassen von Objekten
Einige Objekte in der Anwendung sind greifbar. Dazu gehören zum Beispiel in der Startumgebung die Hocker, die Beistelltische und die beiden Sessel.

Gegriffen werden können die Objekte (Eins pro Hand), indem die Hand für 1,5 Sekunden das Objekt berührt. Ein kreisförmiger Ladebalken zeigt den Fortschritt.

Losgelassen kann das Objekt werden, indem ```Drop``` gesagt wird. Es muss darauf geachtet werden, dass das Wort sehr deutsch ausgesprochen wird, da das Betriebssystem und damit auch die Spracherkennung auf deutsch gestellt sind.

# Technische Dokumentation
## Unity-Version und Packages
Zum Erstellen der Anwendung wurde Unity ```2020.3.21f1```  und das ```CAVE-Asset``` verwendet.
## Herunterladen der Daten
Die Anwendung wird auf dem Gitlab-Server der HTW gehostet. Es ist ein Zugang erforderlich um die Dateien herunterzuladen.
```
Link zu Gitlab
```
## Öffnen des Unity-Projekts
Um das Projekt im Unity-Editor zu öffnen sind folgende Schritte notwendig:
1. Öffnen des Unity-Hubs
2. Hinzufügen eines lokalen Projekts
3. Auswählen des heruntergeladenen Ordners
4. Starten des Projekts aus der Projektliste
## Struktur
### Szenen-Hierarchy
#### Virtual Environment
Enthält den Kinect-Tracker und damit den Kinect-Actor beim Tracking durch die Kinect.
#### House & 3D Furniture
Statische 3D-Objekte ohne Interaktion.
#### Teleport Area
Eine Reihe von Collidern für den Fußboden. Mit diesen Collidern interagiert der Teleport-Pointer. Die GameObjects befinden sich im Layer **Floor**. Um neue Collider hinzuzufügen erstelle einen beliebigen Collider, setze ihn der Übersichtlichkeit halber als Child ders "Teleport Area"-Gameobjects, und ändere den Layer des zugehörigen GameObjects zu ```Floor```.
#### Interactables
Hier sind befinden sich alle greifbaren Objekte. Neue greifbare Objekte sollten der Übersichtlichkeit halber als Kind dieses GameObjects erstellt gesetzt werden. Zwingend notwendig ist das nicht.
#### Interaction
Dieses GameObject enthält alle zur Interaktion benötigten Scripte. Das sind
- KeywordRecognition.cs
- RotatePlayer.cs
- LineRenderer (notwendig für Teleport.cs)
- Teleport.cs

Die Einstellungen und in Uniy zu setzenden Referenzen werden weiter unten im Detail beschrieben.
### Interaktion
#### KeywordRecognition.cs
Die Spracherkennung basiert auf ```using UnityEngine.Windows.Speech;``` und nutzt den Windows-Sprachassistent. Das Script ist sehr simpel aufgebaut. In der Start-Methode wird der Assistent gestartet und es wird auf das Event subscribed, welches ausgelöst wird sobald ein Keyword erkannt wurde.
#### Lokomotion
Für den Teleport soll der Laserstrahl nur angezeigt werden, wenn der Arm gerade ist. Das wurde gelöst, indem das TestActor-Prefab bearbeitet wurde. Am rechten Arm wurde ein isTrigger-Capsule-Collider befestigt, bei welchem geschaut wird, ob Schulter, Ellenbogen und Handgelenk innerhalb sind. Ist dieser Boolean true, wird der Laserstrahl (Linerenderer) angezeigt und das Teleportieren ist freigegeben. Ausgelöst wird es, indem in einer Methode, welche ausgeführt wird sobald das OnKeywordRecognized-Event aufgerufen wurde, abgefragt wird, ob ein Raycast (mit der Layermask auf Floor gesetzt) einen Collider trifft.

Ähnlich ist es bei der Hand-Kopf-Bewegung. Es wird geprüft, ob der hinzugefügte Trigger-Collider der Hand in der Nähe des Kopfes ist und wenn diese Bedingung erfüllt ist, wird ein Raycast geschossen.

Das Rotieren des Spielers erfolgt in einem anderen Script, welches das Camera-Rig dreht, sobald das entsprechende Keyword erkannt wurde.

#### Grab
Der Grab erfolgt mithilfe von zwei Scripten plus einem Visualisierungs-Script für den Ladebalken. In ```InteractableObject.cs``` gibt es eine OnGrab()-Methode, welche beim Grab ausgeführt wird. Anschließend wird die Position auf die der greifenden Hand gesetzt mit einem berechneten Offset, damit das Objekt an der tatsächlich Greif-Stelle gegriffen wird.

Das Script ```GrabInteractor.cs``` ist für die Logik des Greifens zuständig. Es befindet sich an den Händen des TestActors an den GameObjects ```RightHandTipCollider``` und ```LeftHandTipCollider``` . Diese GameObjects haben einen kleinen Sphere-Collider. In dem Script wird geschaut, ob der Collider einen anderen Collider berührt, der das InteractableObject-Script enthält. Ist das der Fall, wird das Objekt nach 1,5 Sekunden gegriffen.

> Ein Beispiel-InteractableObject befindet sich in ```Assets/Prefabs/SampleInteractableObject.prefab```