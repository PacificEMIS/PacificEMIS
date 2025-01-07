import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LovSettingsComponent } from './lov-settings.component';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { IconModule } from '@visurel/iconify-angular';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { SecondaryToolbarModule } from '../../../../@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '../../../../@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '../../../../@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '../../../../@vex/directives/container/container.module';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslateModule } from '@ngx-translate/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {  RouterModule } from '@angular/router';

import { MatTableModule } from '@angular/material/table';
import { SchoolLevelComponent } from '../../list-of-values/school-level/school-level.component';
import { SchoolClassificationComponent } from '../../list-of-values/school-classification/school-classification.component';
import { CountriesComponent } from '../../list-of-values/countries/countries.component';
import { MaleToiletTypeComponent } from '../../list-of-values/male-toilet-type/male-toilet-type.component';
import { MaleToiletAccessibilityComponent } from '../../list-of-values/male-toilet-accessibility/male-toilet-accessibility.component';
import { FemaleToiletTypeComponent } from '../../list-of-values/female-toilet-type/female-toilet-type.component';
import { FemaleToiletAccessibilityComponent } from '../../list-of-values/female-toilet-accessibility/female-toilet-accessibility.component';
import { CommonToiletTypeComponent } from '../../list-of-values/common-toilet-type/common-toilet-type.component';
import { CommonToiletAccessibilityComponent } from '../../list-of-values/common-toilet-accessibility/common-toilet-accessibility.component';
import { RaceComponent } from '../../list-of-values/race/race.component';
import { EthnicityComponent } from '../../list-of-values/ethnicity/ethnicity.component';
import { LanguageComponent } from '../../list-of-values/language/language.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { SharedModuleModule } from '../../../pages/shared-module/shared-module.module';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { DragDropModule } from '@angular/cdk/drag-drop';


@NgModule({
  declarations: [
    LovSettingsComponent,
    SchoolLevelComponent,
    SchoolClassificationComponent,
    CountriesComponent,
    MaleToiletTypeComponent,
    MaleToiletAccessibilityComponent,
    FemaleToiletTypeComponent,
    FemaleToiletAccessibilityComponent,
    CommonToiletTypeComponent,
    CommonToiletAccessibilityComponent,
    RaceComponent,
    EthnicityComponent,
    LanguageComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    IconModule,
    MatButtonModule,   
    MatCardModule,
    MatSidenavModule,
    MatSnackBarModule,
    MatFormFieldModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    ContainerModule,
    MatDatepickerModule,
    MatNativeDateModule,
    TranslateModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    SharedModuleModule,
    MatDialogModule,
    MatMenuModule,
    MatSortModule,
    MatCheckboxModule,
    MatButtonToggleModule,
    MatTooltipModule,
    DragDropModule
  ]
})
export class LovSettingsModule { }
