using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AlterStudentListViewByEnrollmentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string studentListView =
                   @"
					DROP VIEW IF EXISTS student_list_view;

                    CREATE VIEW student_list_view
					AS
					SELECT
  student_master.tenant_id                    AS tenant_id,
  student_master.school_id                    AS school_id,
  student_master.student_id                   AS student_id,
  student_master.alternate_id                 AS alternate_id,
  student_master.district_id                  AS district_id,
  student_master.state_id                     AS state_id,
  student_master.admission_number             AS admission_number,
  student_master.roll_number                  AS roll_number,
  student_master.salutation                   AS salutation,
  student_master.first_given_name             AS first_given_name,
  student_master.middle_name                  AS middle_name,
  student_master.last_family_name             AS last_family_name,
  student_master.suffix                       AS suffix,
  student_master.preferred_name               AS preferred_name,
  student_master.previous_name                AS previous_name,
  student_master.social_security_number       AS social_security_number,
  student_master.other_govt_issued_number     AS other_govt_issued_number,
  student_master.dob                          AS dob,
  student_master.gender                       AS gender,
  student_master.race                         AS race,
  student_master.ethnicity                    AS ethnicity,
  student_master.marital_status               AS marital_status,
  student_master.country_of_birth             AS country_of_birth,
  student_master.nationality                  AS nationality,
  student_master.first_language_id            AS first_language_id,
  student_master.second_language_id           AS second_language_id,
  student_master.third_language_id            AS third_language_id,
  student_master.home_phone                   AS home_phone,
  student_master.mobile_phone                 AS mobile_phone,
  student_master.personal_email               AS personal_email,
  student_master.school_email                 AS school_email,
  student_master.twitter                      AS twitter,
  student_master.facebook                     AS facebook,
  student_master.instagram                    AS instagram,
  student_master.youtube                      AS youtube,
  student_master.linkedin                     AS linkedin,
  student_master.home_address_line_one        AS home_address_line_one,
  student_master.home_address_line_two        AS home_address_line_two,
  student_master.home_address_city            AS home_address_city,
  student_master.home_address_state           AS home_address_state,
  student_master.home_address_zip             AS home_address_zip,
  student_master.bus_no                       AS bus_no,
  student_master.school_bus_pick_up           AS school_bus_pick_up,
  student_master.school_bus_drop_off          AS school_bus_drop_off,
  student_master.mailing_address_same_to_home AS mailing_address_same_to_home,
  student_master.mailing_address_line_one     AS mailing_address_line_one,
  student_master.mailing_address_line_two     AS mailing_address_line_two,
  student_master.mailing_address_city         AS mailing_address_city,
  student_master.mailing_address_state        AS mailing_address_state,
  student_master.mailing_address_zip          AS mailing_address_zip,
  student_master.home_address_country         AS home_address_country,
  student_master.mailing_address_country      AS mailing_address_country,
  student_master.section_id                   AS section_id,
  student_master.student_internal_id          AS student_internal_id,
  student_master.updated_on                   AS updated_on,
  student_master.updated_by                   AS updated_by,
  student_master.enrollment_type              AS enrollment_type,
  student_master.is_active                    AS is_active,
  student_master.student_guid                 AS student_guid,
  student_enrollment.enrollment_id            AS enrollment_id,
  student_enrollment.enrollment_date          AS enrollment_date,
  student_enrollment.enrollment_code          AS enrollment_code,
  student_enrollment.calender_id              AS calender_id,
  student_enrollment.grade_id                 AS grade_id,
  student_enrollment.grade_level_title        AS grade_level_title,
  student_enrollment.rolling_option           AS rolling_option,
  student_enrollment.school_name              AS school_name,
  sections.name                               AS section_name,
  student_master.created_by                   AS created_by,
  student_master.created_on                   AS created_on,
  student_master.student_portal_id            AS student_portal_id,
  student_master.eligibility_504              AS eligibility_504,
  student_master.economic_disadvantage        AS economic_disadvantage,
  student_master.free_lunch_eligibility       AS free_lunch_eligibility,
  student_master.special_education_indicator  AS special_education_indicator,
  student_master.lep_indicator                AS lep_indicator,
  student_master.estimated_grad_date          AS estimated_grad_date
FROM (((student_enrollment
     JOIN student_master
       ON (((student_enrollment.tenant_id = student_master.tenant_id)
            AND (student_enrollment.school_id = student_master.school_id)
            AND (student_enrollment.student_id = student_master.student_id))))
    LEFT JOIN sections
      ON (((student_master.tenant_id = sections.tenant_id)
           AND (student_master.school_id = sections.school_id)
           AND (student_master.section_id = sections.section_id))))
   JOIN (SELECT
           student_enrollment.student_id                AS student_id,
           student_enrollment.school_id                 AS school_id,
           student_enrollment.tenant_id                 AS tenant_id,
           MAX(student_enrollment.enrollment_id)        AS enrollment_id
         FROM student_enrollment
         GROUP BY student_enrollment.tenant_id,student_enrollment.school_id,student_enrollment.student_id) max_enrollment_id)
WHERE ((student_enrollment.student_id = max_enrollment_id.student_id)
       AND (student_enrollment.enrollment_id = max_enrollment_id.enrollment_id)
       AND (student_enrollment.school_id = max_enrollment_id.school_id)
       AND (student_enrollment.tenant_id = max_enrollment_id.tenant_id))";

            migrationBuilder.Sql(studentListView);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string studentListView = @"DROP VIEW student_list_view";
            migrationBuilder.Sql(studentListView);
        }
    }
}
