/* Jeff O'Hara
 * 6/24/2026
 * 
 * This app is a redesigned version of the World Cup Match Tracker. It allows users to track and manage World Cup matches, 
 * including scores, teams, and match details. The app provides a user-friendly interface for easy navigation and interaction.
 */

/* 1500 hrs
 * 7/20/2026
 * 
 * The redesigned World Cup Match Tracker app includes the following features:
 * - Add, edit, and delete matches
 * - View match details and scores
 * - Filter matches by stage (group or knockout)
 * - Display match winners and scores
 * - User-friendly interface for easy navigation
 */

namespace WorldCupMatchTrackerRedesigned;

internal class Program
{
    static void Main(string[] args)
    {
        AppManager app = new AppManager();
        app.Run();
    }
}