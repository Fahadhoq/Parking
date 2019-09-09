using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Parking.Data;


namespace Parking
{
    public partial class MainScreen : Form
    {
        private UserService service;
        private Floor parkingFloor;
        private User oldUser;
        private User member;
        
        

        public MainScreen()
        {
            InitializeComponent();
            service = new UserService();
           

        }

        private void LogInButton_Click(object sender, EventArgs e)
        {
            Admin adminForm = new Admin();
            this.Hide();
            adminForm.ShowDialog();
            this.Show();
        }

        // after taking input from user for parking car check users input and make parking plot visible.
        private void submitButton_Click(object sender, EventArgs e)
        {
            if (nameText.Text.Length!=0 &&  phoneText.Text.Length!=0 && vehicleText.Text.Length!=0 )
            {
                parkingPlotRoot.Visible = true;
                registrationPanel.Visible = false;
                
            }
            else
            {
                MessageBox.Show("Please fill the form correctly");
            }
        }

        // dynamically creates tableLayouts column row accordint to floor
        private void InitTableLayoutPanel(TableLayoutPanel TLP, int rows, int cols)
        {
            rows = (2 * rows) - 1;
            TLP.RowCount = rows;
            TLP.RowStyles.Clear();
            for (int i = 1; i <=rows; i++)
            {
                if((i%2)==0)
                {
                    //create spaces between rows
                    TLP.RowStyles.Add(new RowStyle(SizeType.Percent, 0.45f));
                    
                }
                else
                {
                    TLP.RowStyles.Add(new RowStyle(SizeType.Percent, 1));
                }

            }
            TLP.ColumnCount = cols;
            TLP.ColumnStyles.Clear();
            for (int i = 1; i <= cols; i++)
            {
                TLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 1));
            }
        }


        // dynamically add parking place int parking floor
        private void createParking(List<string> bookedSpaces)
        {
            for (int i = 0; i < parkingPlot.RowCount; i++)
            {
                for (int j = 0; j < parkingPlot.ColumnCount; j++)
                {

                    if (i > 0 && j == 0){ /*empty spaces of entry path */}
                    else if (!(i%2==0)) { /*empty spaces between two rows*/ }
                    else
                    {
                        Button a = new Button();
                        string text = i + "-" + j;
                        if (bookedSpaces.Contains(text))
                        {
                            // create booked buttons
                            a.BackColor = Color.Red;
                            a.Dock = DockStyle.Fill;
                            a.Text = "Booked";
                            a.TextAlign = ContentAlignment.MiddleCenter;
                        }
                        else
                        {
                            //create free space buttons
                            a.BackColor = Color.Blue;
                            a.Dock = DockStyle.Fill;
                            a.Text = text;
                            a.TextAlign = ContentAlignment.MiddleCenter;

                            //creating onclick listener for button
                            a.Click += new EventHandler(parkingPlotClicked);
                        }
                        parkingPlot.Controls.Add(a, j, i);
                    }

                }
            }
        }

        //called whern a parking place is selected by the user
        private void parkingPlotClicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            string name = nameText.Text;
            string phone = phoneText.Text;
            string vehicle = vehicleText.Text;
            DateTime arivalTime = DateTime.Now;

            //creating user according to given data
            User newUser = new User(name, phone, vehicle, arivalTime,btn.Text, parkingFloor.id);

            //booking the space in BookedSpace table and entry the user into UserTable table

            if(service.bookeParkngSpace(newUser,parkingFloor.id))
            {
                //showing confirmation message
                MessageBox.Show("Your parking plot Floor :" + parkingFloor.id + " space: " + btn.Text);
            }
            else
            {
                //showing failure message
                MessageBox.Show("Ops Sorry could not booked you something goes wrong try again later");
            }

            //switching panels to First screen
            firstPanel.Visible = true;
            parkingPlotRoot.Visible = false;
            registrationPanel.Visible = false;

            //clearing parking plot
            parkingPlot.Controls.Clear();

            this.Refresh();

        }


        //called when user want to park button and select park Car button in first screen panel
        private void ParkCarButton_Click(object sender, EventArgs e)
        {
            //get the free space availabe floor from database (ParkingFloors table )
            parkingFloor = service.checkFreeSpace();
         
            if (parkingFloor!=null)
            {

                currentTime.Text = DateTime.Now.ToShortTimeString();
                //get the booked spaces list from database (BookedSpace table )
                List<string> bookedSpaces = service.getFloorsBookedSpace(parkingFloor.id);

                //dynamically creates the table layout colum and rows for visualizing floor
                InitTableLayoutPanel(parkingPlot, parkingFloor.row, parkingFloor.column);

                //create the parking floor with spaces both booked and free 
                createParking(bookedSpaces);

                //setting current TIme in for show
                currentTime.Text = DateTime.Now.ToShortTimeString();

                //hiding and showing container panel
                registrationPanel.Visible = true;
                firstPanel.Visible = false;
            }
            else
            {
                MessageBox.Show("No Free Space For Parking");
            }
        }

//*********MemberParkCar**********
        private void MemberParkCarClick(object sender, EventArgs e)
        {
            //get the free space availabe floor from database (ParkingFloors table )
            parkingFloor = service.checkFreeSpace();

            if (parkingFloor != null)
            {

                MemberCurrentTime.Text = DateTime.Now.ToShortTimeString();
                //get the booked spaces list from database (BookedSpace table )
////                List<string> bookedSpaces = service.getFloorsBookedSpace(parkingFloor.id);

                //dynamically creates the table layout colum and rows for visualizing floor
////                InitTableLayoutPanel(parkingPlot, parkingFloor.row, parkingFloor.column);

                //create the parking floor with spaces both booked and free 
////                createParking(bookedSpaces);

                //setting current TIme in for show
                MemberCurrentTime.Text = DateTime.Now.ToShortTimeString();

                //hiding and showing container panel
                MemberPanel4.Visible = true;
                MemberPanel2.Visible = false;
            }
            else
            {
                MessageBox.Show("No Free Space For Parking");
            }
        }

        //called when user want to take his car from parking plot
        private void takeCarButton_Click(object sender, EventArgs e)
        {
            takingCarPanel.Visible = true;
            firstPanel.Visible = false;
        }

        //called after user give input of his car vehicle no and his phone no as password
        private void carTakingInputSubmitButton_Click(object sender, EventArgs e)
        {
            string vehclNo=takingCarVehicleNotext.Text;
            string phoneNo = takingCarPhoneNoText.Text;
            oldUser = service.unbookCarAndUpdateUserData(vehclNo, phoneNo);
            if (oldUser != null)
            {

                nameLabel.Text = oldUser.name;
                phoneLabel.Text = oldUser.phoneNo;
                vehicleLabel.Text = oldUser.vehicleNo;
                arrivalTimeLabel.Text = oldUser.arrivalTime.ToString();
                dipartingTimeLabel.Text = DateTime.Now.ToString() ;
                parkingDurationLabel.Text = oldUser.getStayinTime().TotalMinutes.ToString("#.##")+"  Minutes";
                
                totalLabel.Text = oldUser.getAmountToBePaid().ToString("#.##")+"$";
                
                paymentSlip.Visible = true;
                takingCarPanel.Visible = false;
            }
            else
            {
                 MessageBox.Show("No car is currently parking in this vehicle no and phone no please recheck the input");
            }
        }

        //called when user click  print button 
        private void printButton_Click(object sender, EventArgs e)
        {
             service.printReciept(oldUser);
             firstPanel.Visible = true;
             parkingPlotRoot.Visible = false;
             registrationPanel.Visible = false;
             paymentSlip.Visible = false;
            oldUser = null;
            this.Refresh();
        }

        private void backButtonOfTakeCar_Click(object sender, EventArgs e)
        {
            firstPanel.Visible = true;
            takingCarPanel.Visible = false;
            this.Refresh();
        }

        private void registrationBackButton_Click(object sender, EventArgs e)
        {
            registrationPanel.Visible = false;
            firstPanel.Visible = true;
            parkingPlot.Controls.Clear();
            this.Refresh();



        }

        private void parkingPlotBackButton_Click(object sender, EventArgs e)
        {
            firstPanel.Visible = true;
            parkingPlotRoot.Visible = false;
            this.Refresh();
        }

//******member*************
        private void MemberLoginClick(object sender, EventArgs e)
        {
            firstPanel.Visible = false;
            MemberPanel1.Visible = true;
            this.Refresh();
        }

        private void Member1BackButton(object sender, EventArgs e)
        {
            firstPanel.Visible = true;
            MemberPanel1.Visible = false;
            this.Refresh();
        }

        private void MemberSubmitButtonClick(object sender, EventArgs e)
        {
           User member = new User();

            member.name =MemberNameText.Text;
            member.phoneNo = MemberPasswordTextBox.Text;

            UserService Member = new UserService();
            if (Member.UserLoginValidation(member))
            {
                MemberPanel2.Visible = true;
                MemberPanel1.Visible = false;
                this.Refresh();
            }
            else
            {
                MessageBox.Show("Invalid Id or Password", "Login Failed");
            }
        }

        private void MemberPanel2BackButton(object sender, EventArgs e)
        {
            
            MemberPanel2.Visible = false;
            MemberPanel1.Visible = true;
            this.Refresh();
        }



        //**************************member*******************



      

        private void MemberTakeCarClick(object sender, EventArgs e)
        {
            takingCarPanel.Visible = true;
            firstPanel.Visible = false;
            this.Refresh();
        }

        private void MemberRagBackButton(object sender, EventArgs e)
        {
            MemberPanel2.Visible = true;
            MemberPanel4.Visible = false;
            this.Refresh();
        }

        private void SubmitButtonMemberReg(object sender, EventArgs e)
        {
            if (MemberNameText.Text.Length != 0 && MemberPhoneText.Text.Length != 0 && MemberVehicleText.Text.Length != 0)
            {
                MemberPanel3.Visible = true;
                MemberPanel4.Visible = false;

            }
            else
            {
                MessageBox.Show("Please fill the form correctly");
            }
        }

        private void MemberParkPlotButton(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            string name = MemberNameText.Text;
            string phone = MemberPhoneText.Text;
            string vehicle = MemberVehicleText.Text;
            DateTime arivalTime = DateTime.Now;

            //creating user according to given data
            User newUser = new User(name, phone, vehicle, arivalTime, btn.Text, parkingFloor.id);

            //booking the space in BookedSpace table and entry the user into UserTable table

            if (service.bookeParkngSpace(newUser, parkingFloor.id))
            {
                //showing confirmation message
                MessageBox.Show("Your parking plot Floor :" + parkingFloor.id + " space: " + btn.Text);
            }
            else
            {
                //showing failure message
                MessageBox.Show("Ops Sorry could not booked you something goes wrong try again later");
            }

            //switching panels to First screen
            firstPanel.Visible = true;
            MemberPanel3.Visible = false;
            MemberPanel4.Visible = false;

            //clearing parking plot
////            parkingPlot.Controls.Clear();

            this.Refresh();
        }
    }


}
