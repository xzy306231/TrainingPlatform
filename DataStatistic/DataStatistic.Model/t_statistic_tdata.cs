using System;
using System.Collections.Generic;


public partial class t_statistic_tdata
{
    public long id { get; set; }
    public int training_tnum { get; set; }
    public int training_pnum { get; set; }
    public int intraining_pnum { get; set; }
    public int comtraining_pnum { get; set; }
    public decimal comtraining_rate { get; set; }
    public decimal learning_comrate { get; set; }
    public decimal max_learningtime { get; set; }
    public decimal min_learningtime { get; set; }
    public decimal avg_learningtime { get; set; }
    public decimal sum_learningtime { get; set; }
    public decimal task_comrate { get; set; }
    public decimal task_passrate { get; set; }
    public decimal avg_tasktime { get; set; }
    public decimal sum_tasktime { get; set; }
    public decimal subject_comrate { get; set; }
    public decimal subject_passrate { get; set; }
    public decimal exam_subrate { get; set; }
    public decimal exam_rightrate { get; set; }
    public decimal exam_passrate { get; set; }
    public sbyte? delete_flag { get; set; }
    public DateTime? t_create { get; set; }
    public DateTime? t_modified { get; set; }
}

