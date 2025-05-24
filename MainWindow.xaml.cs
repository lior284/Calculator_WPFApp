using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Calculator_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double? firstNumber; // The number that is used in the FirstDisplay
        double? secondNumber; /* The variable used to save the number that last was used (but then replaced). Most of the time its
                              * the number used in the SecondDisplay */
        double? numBefSpeOpe; /* Number before special operation (used to save the number before using (for instance) sqrt)
                               * ==> For example: saving the number 9 after using sqrt on it and making firstNumber 3. */
        char? operation; // The variable used to save the current operation
        bool operationActive; // Indicates if the last button the user pressed is operation (meaning the operation is displayed)
        bool afterCalc; // Indicates if the displayed number was calculated (by equlas)
        bool afterSpeOpe; // Indicates if the displayed number was calculated (by a special operation)

        public MainWindow()
        {
            InitializeComponent();

            Start();
        }

        private void Start()
        {
            // Initializing all variables
            firstNumber = 0;
            secondNumber = null;
            numBefSpeOpe = null;
            operation = null;
            operationActive = false;
            afterCalc = false;
            afterSpeOpe = false;
        }

        private void EquBtn_Click(object sender, RoutedEventArgs e)
        {
            if(!operationActive)
            {
                /* If the operationActive is false then there are two options:
                 * 1. The user inputed a number and then pressed equals.
                 * 2. The user inputed a number, pressed an operation, inputed another number and then pressed equals.
                 * For option 1 I do nothing (like on iOS)*, and for option 2 I actually calculate.
                 * 
                 * *In windows it does does something but I chose not to do anything.
                 */
                if(SecondDisplay.Text != null)
                {
                    /* If the user is after a special operation calculation (for example pressed sqrt on 81 and it shows to him 9) and he
                     * pressed the equBtn then I don't want to do anything (like in iOS*)
                     * Although, if the user is mid operation then I do need to calculate the result (for example 5 * sqrt(9) = )
                     * 
                     * *In windows it changes a bit but I chose not to do it.
                     */
                    if(afterSpeOpe && operation == null)
                    {
                        return;
                    }
                    /* If afterCalc is true then I need to update the SecondDisplay to show the new expression (with the result of the last
                     * calculation). The rest of the code doesn't need a change */
                    if(afterCalc)
                    {
                        SecondDisplay.Text = secondNumber + " " + operation;
                    }
                    // Updating the second display (from (for example) "5 +" to "5 + 3 =")
                    SecondDisplay.Text += " " + firstNumber + " =";
                    // Calculating the result
                    switch(operation)
                    {
                        case '+':
                            secondNumber += firstNumber;
                            FirstDisplay.Text = secondNumber.ToString();
                            break;
                        case '-':
                            secondNumber -= firstNumber;
                            FirstDisplay.Text = secondNumber.ToString();
                            break;
                        case 'x':
                            secondNumber *= firstNumber;
                            FirstDisplay.Text = secondNumber.ToString();
                            break;
                        case '÷':
                            secondNumber /= firstNumber;
                            FirstDisplay.Text = secondNumber.ToString();
                            break;
                    }
                    afterCalc = true;
                    afterSpeOpe = false;
                }
            }
        }

        private void PluMin_Click(object sender, RoutedEventArgs e)
        {
            if(!operationActive)
            {
                if(afterCalc || afterSpeOpe)
                {
                    SecondDisplay.Text = null;
                    firstNumber = secondNumber;
                    secondNumber = null;
                }
                firstNumber = -firstNumber;
                FirstDisplay.Text = firstNumber.ToString();

                afterCalc = false;
                afterSpeOpe = false;
            }
        }
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            Button clickedNumberBtn = (Button) sender;
            if(operationActive)
            {
                SecondDisplay.Text += " " + operation;

                firstNumber = Convert.ToDouble(clickedNumberBtn.Content);
                FirstDisplay.Text = firstNumber.ToString();
            } else
            {
                /* Three options here:
                 * 1. The displayed number is zero (not as the result after calculation, but as the default 
                 *      number displayed) (need to replace it with a new number).
                 * 2. The displayed number is a number being build (need to add to the end of it the new digit). It can mean both a regular number or a the user last pressed point.
                 * 3. The displayed number is a calculated number (need to replace it with a new number)
                 * 
                 * When the user uses a special operation (for instance sqrt) then nothing is displayed in the second
                 * display, so if the user presses a number btn right after that then we're also in option 3.
                 * 
                 * Originaly I thought that I had a problem recognizing the different zeros in the first nubmer - there is the
                 * default one and there is the one that is the result of a calculation. Then I realized that in both cases
                 * I want to delete everything in the second display, and to replace the first display to the new number.
                 * There is also the possiblity that the user has pressed zero to start a new number and they will press another
                 * number right after it, building a number (like the number 0234). But in that case I can replace the zero entirely
                 * with the number after it because adding a zero to the start of any number doens't change at all the number it self,
                 * meaning 0234 = 234.
                 */

                // This if body is for both options 1 & 3
                if((firstNumber == 0 && FirstDisplay.Text == "0") || afterCalc || afterSpeOpe)
                {
                    firstNumber = Convert.ToDouble(clickedNumberBtn.Content);
                    FirstDisplay.Text = firstNumber.ToString();
                    /* If the user is mid-operation and he presses 0 and then (for example) 2 then I don't want to delete the SecondDisplay
                     * - I just want to replace the 0 with 2.
                     */
                    if(afterCalc || afterSpeOpe)
                    {
                        secondNumber = null;
                        SecondDisplay.Text = secondNumber.ToString();
                    }
                    
                } else /* This else body is for option 2 */
                {
                    /* Unlike other places, here I update the FirstDisplay first and then the firstNuber becuase of the option that
                     * the user pressed the point last and it can lead to bugs if I update the firstNumber first. */
                    FirstDisplay.Text += clickedNumberBtn.Content;
                    firstNumber = Convert.ToDouble(FirstDisplay.Text);
                }
            }
            afterCalc = false;
            afterSpeOpe = false;
            operationActive = false;
        }

        private void Point_Click(object sender, RoutedEventArgs e)
        {
            if(!operationActive)
            {
                if(!afterCalc && !afterSpeOpe)
                {
                    // Only if the user hasn't already added a dot to the number they can add one.
                    if(!FirstDisplay.Text.Contains('.'))
                    {
                        FirstDisplay.Text += ".";
                        // Doesn't need to update firstNumber
                    }
                } else // If the user is after calculation I want to replace the entire number to "0." - not just add the dot, and delete the second display
                {
                    secondNumber = null;
                    SecondDisplay.Text = secondNumber.ToString();

                    FirstDisplay.Text = "0.";
                    firstNumber = 0.0;
                }
                afterCalc = false;
                afterSpeOpe = false;
            }
        }

        #region Delete Buttons
        private void CE_Click(object sender, RoutedEventArgs e)
        {
            if(!operationActive)
            { 
                if(afterCalc || afterSpeOpe)
                {
                    C_Click(sender, e);
                } else
                {
                    firstNumber = 0;
                    FirstDisplay.Text = firstNumber.ToString();
                }
            }
        }

        private void C_Click(object sender, RoutedEventArgs e)
        {
            // Reseting everything
            firstNumber = 0;
            FirstDisplay.Text = "0";
            secondNumber = null;
            SecondDisplay.Text = null;
            numBefSpeOpe = null;
            operation = null;
            operationActive = false;
            afterCalc = false;
            afterSpeOpe = false;
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if(!operationActive)
            {
                if(afterCalc || afterSpeOpe)
                {
                    C_Click(sender, e);
                } else
                {
                    if(FirstDisplay.Text != "0")
                    {
                        // [..^1] means the whole string except the last char
                        FirstDisplay.Text = FirstDisplay.Text[..^1];
                        /* If the last char is now a dot then the firstNumber can't hold it like this:
                         * firstNumber = Convert.ToDouble(FirstDisplay.Text); (it will be an error),
                         * so I'm saving in it the number without the dot.
                         */
                        if(FirstDisplay.Text[^1] == '.')
                        {
                            firstNumber = Convert.ToDouble(FirstDisplay.Text[..^1]);
                        } else
                        {
                            firstNumber = Convert.ToDouble(FirstDisplay.Text);
                        }
                    }
                }
            }
        }
        #endregion

        #region Operations Buttons
        private void Precent_Click(object sender, RoutedEventArgs e)
        {
            if(!operationActive)
            {
                numBefSpeOpe = firstNumber;
                // If the user has a number in the variable secondNumber then I calculate the precent from this number.
                if(secondNumber != null && !afterCalc)
                {
                    /*
                     * In calculators, if the user uses addition or substraction then the percentage is calculated out of the first number,
                     * but if the user uses multiplication or division then the percentage is calculated out of 1 (meaning 20% = 0.2).
                     * So I calculate it differently based on the operation.
                     * I know there was an operation because the second display isn't null.
                     */
                    if(operation == '+' || operation == '-')
                    {
                        firstNumber = Convert.ToDouble(secondNumber * (firstNumber / 100));
                    } else
                    {
                        firstNumber /= 100;
                    }
                } else /* If the user doens't have a number in the variable secondDisplay then I just right precent/100. Basically the same calculation if the second number is 1. */
                {
                    firstNumber /= 100;
                    SecondDisplay.Text = null; // Not already null if entering this else becuase of "!afterCalc = false".
                }
                FirstDisplay.Text = firstNumber.ToString(); // Updating the FirstDisplay after the firstNumber change.
                afterSpeOpe = true;
                afterCalc = false;
            }
        }
        
        private void SpecialOperationExcludingPrecent_Click(object sender, RoutedEventArgs e)
        {
            if(!operationActive)
            {
                if(afterCalc)
                {
                    firstNumber = secondNumber;
                }
                numBefSpeOpe = firstNumber;
                switch(((Button) sender).Content.ToString())
                {
                    case "1 / x":
                        firstNumber = 1 / firstNumber;
                        break;
                    case "x^2":
                        firstNumber = Math.Pow(Convert.ToDouble(firstNumber), 2);
                        break;
                    case "√x":
                        firstNumber = Math.Sqrt(Convert.ToDouble(firstNumber));
                        break;
                }
                FirstDisplay.Text = firstNumber.ToString();

                /* The number stored in secondNumber is the result of the operation (becuase if I use it I will need the actual number).
                 * But the SecondDisplay is showing the 'fancy' number (with the √ or ^2 or 1/ sign) so that it is more intuitive to the
                 * user. */
                if(operation == null || afterCalc || afterSpeOpe) // If I'm mid-operaion then I don't want to change the second number
                {
                    secondNumber = firstNumber;
                    switch(((Button)sender).Content.ToString())
                    {
                        case "1 / x":
                            SecondDisplay.Text = "1/" + numBefSpeOpe;
                            break;
                        case "x^2":
                            SecondDisplay.Text = numBefSpeOpe + "^2";
                            break;
                        case "√x":
                            SecondDisplay.Text = "√" + numBefSpeOpe;
                            break;
                    }
                }

                afterSpeOpe = true;
                afterCalc = false;
            }
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            /*
             * Before the user presses the operation button there are two options - either the operationActive
             * is true or false.
             * In both cases I want the SecondDisplay to display the firstNumber, the FirstDisplay to display
             * the operation, and of course the variable operation to change to the pressed operation and
             * operationActive to change to true.
            */

            // Although, if the operationActive is true and I do this line: secondNumber = firstNumber; then the second number would change to null and destroy the number it stored.
            if(!operationActive)
            {
                // If afterCalc is true then I don't want to update secondNumber to be firstNumber because secondNumber is already what it needs to be
                if(!afterCalc)
                {
                    /* The if is for the option that the user calculates a 3-elements calculation (for example presses 3 then + then 5 then +
                     * again) In this case, I want to calculate the first part and then continue with the result of it -- that's why I call the
                     * equBtn function (to calculate the result) and then I continue on with the second operation the user pressed.
                     * It will work exactly like if the user presses calculates something and immediately after presses operation.
                     */
                    if(secondNumber != null)
                    {
                        EquBtn_Click(sender, e);
                    } else
                    {
                        secondNumber = firstNumber;
                    }
                }
                SecondDisplay.Text = secondNumber.ToString();
            }

            Button clickedOperationBtn = (Button)sender;
            operation = Convert.ToChar(clickedOperationBtn.Content.ToString() ?? ""); // I added the ?? " " just to not see the possible null warning

            firstNumber = null;
            FirstDisplay.Text = operation.ToString();
            operationActive = true;

            afterCalc = false;
            afterSpeOpe = false;
        }
        #endregion
    }
}