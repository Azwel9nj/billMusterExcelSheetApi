using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System;
using System.IO;
using System.IO;
using System.Net;
using System.Linq;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Assign01
{
    class Program 
    {  
        
        static void Main(string[] args)
        {
            string line;
            List<object> sendData = new List<object>();
            int failCount = 0;
            string transactions = "CONSOLIDATED 2023 BILLMUSTER";
            string failed = "FAILEDTRANSACTIONS "+transactions;            
            
            FileStream f = new FileStream(failed+".csv", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            int verifyRows = 0;
            
            StreamWriter s = new StreamWriter(f);
            string instid = "INSTID";
            string xref = "XREF";
            string fccref = "FCCREF";
            string consno =	"CONSNO";	
            string consname = "CONSNAME";	
            string billno = "BILLNO";
            string txndate = "TXNDATE";
            string bamt = "BAMT";	
            string bccy = "BCCY";	
            string ofsacno = "OFSACNO";	

            s.WriteLine(instid+","+xref +","+ fccref +","+ consno +","+ consname +","+ billno +","+ txndate +","+ bamt +","+bccy+","+ofsacno);
            using (StreamReader r = new StreamReader(transactions+".csv"))
            {
                string currentLine;
                string[] strlist;
                
                char[] separator = {','};
                // currentLine will be null when the StreamReader reaches the end of file
                //string headerLine = r.ReadLine();
                
                while((currentLine = r.ReadLine()) != null &&verifyRows < 10595)
                {
                    if (verifyRows == 0){
                        line = r.ReadLine();
                    }else{
                       line = currentLine.ToString(); 
                    }                    
                    verifyRows +=1 ;                    
                    //line = currentLine.ToString();
                    
                    strlist = line.Split(',',StringSplitOptions.None);
                    string requestid = (strlist[1]);
                    string name = strlist[4];
                    string studentid = (strlist[5]);
                    string amount = (strlist[7]);
                    string tranid = strlist[6];
                    string type = "L";
                    string date = strlist[3];           

                    //Console.WriteLine(line);

                    // if(condition == 8){
                    //     inputFormat = "M/d/yyyy";
                    // }else if(condition == 9){
                    //         int position = inputDate.IndexOf("/");
                    //     if(position == 1){
                    //         inputFormat = "M/dd/yyyy";
                    //     }else{
                    //         inputFormat = "MM/d/yyyy";
                    //     }
                    // }else{
                    //     inputFormat = "MM/dd/yyyy";
                    // }

                    // DateTime outputDate = DateTime.ParseExact(inputDate, inputFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    // string date = outputDate.ToString(outputFormat);

                    
                    // string jsonData = @"{
                    // 'StentId': '" + studentid + "','txnId': '" + tranid + "','txnDate': " + date + ", 'amount': " + amount + "}";        
                    
                    try{
                        Console.WriteLine(requestid+" "+tranid +" "+ date +"  "+ type +" "+ studentid +" "+ name +" "+ amount+ " "+ verifyRows);
                        //Console.WriteLine(amount);
                        s.WriteLine(requestid+","+tranid +","+ date +","+ type +","+ studentid +","+ name +","+ amount +","+ verifyRows);
                    }catch(Exception e){
                        failCount += 1;
                        Console.WriteLine("ERROR FAIL" +failCount);
                    }                  
                   
                    // try{
                    //     SendDataToAPI(requestid,  tranid , date, amount, type, studentid, name, verifyRows);
                    // }catch(Exception e){
                    //     failCount += 1;
                    //     Console.WriteLine("FAIL UNSUCCESSFUL"+" "+failCount+" "+"AT :"+" "+verifyRows+" "+ "TRANSACTION:"+" "+ tranid);
                    //     s.WriteLine(requestid+","+tranid +","+ date +","+ type +","+ studentid +","+ name +","+ amount +","+ verifyRows);                       
                    // }                  
                }
            }
            //r.Close();
            s.Close();
            f.Close();
        }

        static void SendDataToAPI(string requestid, string transactionId,string date, string amount,string type, string studentid,string name, int verifyRows)
        {
            using (WebClient client = new WebClient())
            {
                var key = "9695f71b8c07d7649cdb6901d3ba4f18f4ae696d";
                var plainText = key + transactionId;
                var data = Encoding.UTF8.GetBytes(plainText);
                //client.Timeout = 10000;
                //client.Timeout = Timeout;
                var sha1 = new SHA1CryptoServiceProvider();
                var sha1data = sha1.ComputeHash(data);
                var hashed = Convert.ToBase64String(sha1data);
                
                string url = $"https://edurole.lmmu.ac.zm/api/transactions/postTran?RequestId={requestid}&TranID={transactionId}&key={hashed}&Date={date}&Amount={amount}&Type={type}&StudentID={studentid}&Name={name}";
                string response = client.DownloadString(url);
                Console.WriteLine("Response from API: " + response +" "+ verifyRows);
            }
        }
    }
}