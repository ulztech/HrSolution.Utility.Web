# HRIS UIXE Solution

## Domain Models

### Login User
    public class UserModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }
    }


### Employee Master 

    public class hremp
    {
        [Key]
        [Required]
        [Display(Name = "RefId")]
        public int hremp_id { get; set; }

        [Required]
        [Display(Name = "ID")]
        [StringLength(20, ErrorMessage = "The {0} must be minimum of {2} characters long.", MinimumLength = 4)]
        public string hremp_code { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(100, ErrorMessage = "The {0} must be minimum of {2} characters long.", MinimumLength = 2)]
        public string hremp_lname { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(100, ErrorMessage = "The {0} must be minimum of {2} characters long.", MinimumLength = 1)]
        public string hremp_fname { get; set; }

        [Display(Name = "Middle Name")]
        public string hremp_mname { get; set; }

        [Display(Name = "Nickname")]
        public string hremp_nickname { get; set; }

        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? hremp_birthdate { get; set; }

        [Display(Name = "Country")]
        public int hremp_country_id { get; set; }

        [Display(Name = "Religion")]
        public int hremp_religion_id { get; set; }

        [Display(Name = "Blood Type")]
        public string hremp_blood_type { get; set; }

        [Display(Name = "Height")]
        public string hremp_height { get; set; }

        [Display(Name = "Weight")]
        public string hremp_weight { get; set; }

        [Display(Name = "Gender")]
        public string hremp_gender { get; set; }

        [Display(Name = "Civil Status")]
        public string hremp_civil_status { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string hremp_email { get; set; }

        [Display(Name = "Phone")]
        public string hremp_phone { get; set; }

        [Display(Name = "Remarks")]
        public string hremp_remarks { get; set; }

        [Display(Name = "Birth Place")]
        public string hremp_birthplace { get; set; }

        [Display(Name = "Ethnic")]
        public int hremp_ethnic_id { get; set; }

        public DateTime hremp_created { get; set; }

        [MaxLength(50)]
        public string hremp_createdby { get; set; }

        [Display(Name = "Apply Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? hremp_applydate { get; set; }

        public Guid? hremp_users_pid { get; set; }

        [Display(Name = "Name")]
        public virtual string hremp_name
        {
            get
            {
                string text = hremp_lname + ", " + hremp_fname + " " + hremp_mname;
                return text.Trim().ToUpper();
            }
        }

        public string hremp_imageid { get; set; }

        public ICollection<hremppos> hremppos { get; set; }

        public ICollection<hrempsal> hrempsal { get; set; }

        public ICollection<hrempgv> hrempgv { get; set; }

        public ICollection<hrempshift> hrempshift { get; set; }

        public ICollection<hrapp> hrapp { get; set; }

        [Display(Name = "Name")]
        public virtual string hremp_name_code
        {
            get
            {
                string text = hremp_lname + ", " + hremp_fname + " " + hremp_mname + " [" + hremp_code + "]";
                return text.Trim().ToUpper();
            }
        }

        public virtual string BiomatrixID
        {
            get
            {
                string text = "";
                try
                {
                    text = hremppos.First()?.hremppos_biomatrix ?? "";
                }
                catch
                {
                    text = "";
                }

                return (text.Length > 0) ? text : hremp_code;
            }
        }

        public virtual ICollection<hrempshift> hrempshiftND { get; set; }
    } 
    
### Time Logs

    public class Hrtimelog
    {
        public int HrtimelogId { get; set; }

        public int HrtimelogHrempId { get; set; }

        public DateTime HrtimelogDatein { get; set; }

        public TimeSpan HrtimelogTimein { get; set; }

        public DateTime? HrtimelogDateout { get; set; }

        public TimeSpan? HrtimelogTimeout { get; set; }

        public string HrtimelogCreatedby { get; set; } = null;


        public DateTime HrtimelogCreated { get; set; }

        public string? HrtimelogEditedby { get; set; }

        public DateTime? HrtimelogEdited { get; set; }

        public bool? HrtimelogPosted { get; set; }
    }
 
 
 
