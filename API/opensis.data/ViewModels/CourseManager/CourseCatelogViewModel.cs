/***********************************************************************************
openSIS is a free student information system for public and non-public
schools from Open Solutions for Education, Inc.Website: www.os4ed.com.

Visit the openSIS product website at https://opensis.com to learn more.
If you have question regarding this software or the license, please contact
via the website.

The software is released under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, version 3 of the License.
See https://www.gnu.org/licenses/agpl-3.0.en.html.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Copyright (c) Open Solutions for Education, Inc.

All rights reserved.
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.CourseManager
{
    public class CourseCatelogViewModel : CommonFields
    {
        public CourseCatelogViewModel()
        {
            CourseWithCourseSectionDetailsViewModels = new List<CourseWithCourseSectionDetailsViewModel>();
        }
        public List<CourseWithCourseSectionDetailsViewModel> CourseWithCourseSectionDetailsViewModels { get; set; }
        public Guid? TenantId { get; set; }
        public int? SchoolId { get; set; }
        public string? CourseSubject { get; set; }
        public int? CourseId { get; set; }
        public string? MarkingPeriodId { get; set; }
        public string? GradeLevel { get; set; }
        public int? RoomId { get; set; }
        public int? PeriodId { get; set; }
        public decimal? AcademicYear { get; set; }

        public string? SchoolName { get; set; }
        public string? StreetAddress1 { get; set; }
        public string? StreetAddress2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? District { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
        public string? SchoolYearStartDate { get; set; }
        public string? SchoolYearEndDate { get; set; }
        public string? MarkingPeriodTitle { get; set; }
        public string? GradeLevelTitle { get; set; }
    }
}
